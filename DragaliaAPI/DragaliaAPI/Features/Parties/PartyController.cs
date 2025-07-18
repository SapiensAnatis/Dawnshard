using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Parties;

[Route("party")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PartyController(
    IPartyRepository partyRepository,
    IUnitRepository unitRepository,
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    ILogger<PartyController> logger,
    IPartyPowerService partyPowerService,
    IPartyPowerRepository partyPowerRepository,
    IMissionProgressionService missionProgressionService,
    ApiContext apiContext
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

        logger.LogDebug(
            "Received party update request: {@request}",
            requestParty.RequestPartySettingList
        );

        List<Charas> selectedCharas = requestParty
            .RequestPartySettingList.Where(x => x.CharaId != 0)
            .Select(y => y.CharaId)
            .ToList();
        List<long> selectedDragons = requestParty
            .RequestPartySettingList.Where(x => x.EquipDragonKeyId != 0)
            .Select(y => (long)y.EquipDragonKeyId)
            .ToList();

        int ownedCharaCount = await apiContext.PlayerCharaData.CountAsync(
            x => selectedCharas.Contains(x.CharaId),
            cancellationToken
        );
        int ownedDragonCount = await unitRepository.Dragons.CountAsync(
            x => selectedDragons.Contains(x.DragonKeyId),
            cancellationToken
        );

        if (ownedCharaCount < selectedCharas.Count || ownedDragonCount < selectedDragons.Count)
        {
            logger.LogError(
                "Party update validation failed. Party unit count: {PartyUnitCount}; owned chara count: {OwnedCharaCount}; owned dragon count {OwnedDragonCount}",
                requestParty.RequestPartySettingList.Count,
                ownedCharaCount,
                ownedDragonCount
            );
            throw new DragaliaException(ResultCode.PartySwitchSettingCharaShort);
        }

        int partyPower = await partyPowerService.CalculatePartyPower(
            requestParty.RequestPartySettingList
        );

        await partyPowerRepository.SetMaxPartyPowerAsync(partyPower);
        missionProgressionService.OnPartyPowerReached(partyPower);

        logger.LogTrace("Party power {power}", partyPower);

        DbParty dbEntry = new DbParty()
        {
            ViewerId = this.ViewerId,
            PartyNo = requestParty.PartyNo,
            PartyName = requestParty.PartyName,
            Units = requestParty
                .RequestPartySettingList.Select(x =>
                    x.MapToDbPartyUnit(this.ViewerId, requestParty.PartyNo)
                )
                .ToList(),
        };

        await partyRepository.SetParty(dbEntry);

        if (requestParty.IsEntrust)
        {
            missionProgressionService.OnPartyOptimized(requestParty.EntrustElement);
        }

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Returning updated party list: {@list}", updateDataList.PartyList);

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
}
