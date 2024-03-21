using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.PartyPower;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("party")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PartyController(
    IPartyRepository partyRepository,
    IUnitRepository unitRepository,
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IMapper mapper,
    ILogger<PartyController> logger,
    IPartyPowerService partyPowerService,
    IPartyPowerRepository partyPowerRepository,
    IMissionProgressionService missionProgressionService
) : DragaliaControllerBase
{
    /// <summary>
    /// Does not seem to do anything useful.
    /// ILSpy indicates the response should contain halidom info, but it is always empty and only called on fresh accounts.
    /// </summary>
    [HttpPost("index")]
    public DragaliaResult Index()
    {
        return this.Ok(new PartyIndexResponse(new List<BuildList>()));
    }

    [HttpPost("set_party_setting")]
    public async Task<DragaliaResult> SetPartySetting(
        PartySetPartySettingRequest requestParty,
        CancellationToken cancellationToken
    )
    {
        // Validate the player owns all the entities they have requested to add
        // TODO: Weapon validation
        // TODO: Amulet validation
        // TODO: Talisman validation
        // TODO: Shared skill validation

        foreach (PartySettingList partyUnit in requestParty.RequestPartySettingList)
        {
            if (
                !await this.ValidateCharacterId(partyUnit.CharaId)
                || !await this.ValidateDragonKeyId(partyUnit.EquipDragonKeyId)
            )
            {
                throw new DragaliaException(ResultCode.PartySwitchSettingCharaShort);
            }
        }

        int partyPower = await partyPowerService.CalculatePartyPower(
            requestParty.RequestPartySettingList
        );

        await partyPowerRepository.SetMaxPartyPowerAsync(partyPower);
        missionProgressionService.OnPartyPowerReached(partyPower);

        logger.LogTrace("Party power {power}", partyPower);

        DbParty dbEntry = mapper.Map<DbParty>(
            new PartyList(
                requestParty.PartyNo,
                requestParty.PartyName,
                requestParty.RequestPartySettingList
            )
        );

        await partyRepository.SetParty(dbEntry);

        if (requestParty.IsEntrust)
        {
            missionProgressionService.OnPartyOptimized(requestParty.EntrustElement);
        }

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new PartySetPartySettingResponse(updateDataList, new()));
    }

    [HttpPost("set_main_party_no")]
    public async Task<DragaliaResult> SetMainPartyNo(
        PartySetMainPartyNoRequest request,
        CancellationToken cancellationToken
    )
    {
        await userDataRepository.SetMainPartyNo(request.MainPartyNo);

        await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new PartySetMainPartyNoResponse(request.MainPartyNo));
    }

    [HttpPost("update_party_name")]
    public async Task<DragaliaResult> UpdatePartyName(
        PartyUpdatePartyNameRequest request,
        CancellationToken cancellationToken
    )
    {
        await partyRepository.UpdatePartyName(request.PartyNo, request.PartyName);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new PartyUpdatePartyNameResponse() { UpdateDataList = updateDataList });
    }

    private async Task<bool> ValidateCharacterId(Charas id)
    {
        if (id == Charas.Empty)
            return true;

        // TODO: can make this single query instead of 8 (this method is called in a loop)
        IEnumerable<Charas> ownedCharaIds = await unitRepository
            .Charas.Select(x => x.CharaId)
            .ToListAsync();

        Charas c = (Charas)Enum.ToObject(typeof(Charas), id);

        if (!ownedCharaIds.Contains(c))
        {
            logger.LogError(
                "Request from DeviceAccount {account} contained not-owned character id {id}",
                DeviceAccountId,
                id
            );
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateDragonKeyId(ulong keyId)
    {
        // Empty slot
        if (keyId == 0)
            return true;

        IEnumerable<long> ownedDragonKeyIds = await unitRepository
            .Dragons.Select(x => x.DragonKeyId)
            .ToListAsync();

        if (!ownedDragonKeyIds.Contains((long)keyId))
        {
            logger.LogError(
                "Request from DeviceAccount {id} contained not-owned dragon_key_id {key_id}",
                DeviceAccountId,
                keyId
            );
            return false;
        }

        return true;
    }
}
