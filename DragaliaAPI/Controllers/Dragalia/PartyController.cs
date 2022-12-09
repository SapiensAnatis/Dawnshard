using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("party")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PartyController : DragaliaControllerBase
{
    private readonly IPartyRepository partyRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;
    private readonly ISessionService sessionService;
    private readonly ILogger<PartyController> logger;

    public PartyController(
        IPartyRepository partyRepository,
        IUnitRepository unitRepository,
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService,
        IMapper mapper,
        ISessionService sessionService,
        ILogger<PartyController> logger
    )
    {
        this.partyRepository = partyRepository;
        this.unitRepository = unitRepository;
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
        this.sessionService = sessionService;
        this.logger = logger;
    }

    /// <summary>
    /// Does not seem to do anything useful.
    /// ILSpy indicates the response should contain halidom info, but it is always empty and only called on fresh accounts.
    /// </summary>
    [HttpPost("index")]
    public DragaliaResult Index()
    {
        return this.Ok(new PartyIndexData(new List<BuildList>()));
    }

    [HttpPost("set_party_setting")]
    public async Task<DragaliaResult> SetPartySetting(PartySetPartySettingRequest requestParty)
    {
        // Validate the player owns all the entities they have requested to add
        // TODO: Weapon validation
        // TODO: Amulet validation
        // TODO: Talisman validation
        // TODO: Shared skill validation

        foreach (PartySettingList partyUnit in requestParty.request_party_setting_list)
        {
            if (
                !await this.ValidateCharacterId(partyUnit.chara_id, this.DeviceAccountId)
                || !await this.ValidateDragonKeyId(
                    partyUnit.equip_dragon_key_id,
                    this.DeviceAccountId
                )
            )
            {
                return this.BadRequest();
            }
        }

        // This is ugly but if I do it inline with the .Map() call, wyrmprints don't get saved????
        // Automapper bug possibly
        PartyList requestParty2 =
            new(
                requestParty.party_no,
                requestParty.party_name,
                requestParty.request_party_setting_list
            );

        DbParty dbEntry = mapper.Map<DbParty>(requestParty2);

        await partyRepository.SetParty(this.DeviceAccountId, dbEntry);
        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );
        await partyRepository.SaveChangesAsync();

        return this.Ok(new PartySetPartySettingData(updateDataList, new()));
    }

    [HttpPost("set_main_party_no")]
    public async Task<DragaliaResult> SetMainPartyNo(PartySetMainPartyNoRequest request)
    {
        await this.userDataRepository.SetMainPartyNo(this.DeviceAccountId, request.main_party_no);
        await this.userDataRepository.SaveChangesAsync();

        return this.Ok(new PartySetMainPartyNoData(request.main_party_no));
    }

    private async Task<bool> ValidateCharacterId(Charas id, string deviceAccountId)
    {
        if (id == Charas.Empty)
            return true;

        IEnumerable<Charas> ownedCharaIds = await this.unitRepository
            .GetAllCharaData(deviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        Charas c = (Charas)Enum.ToObject(typeof(Charas), id);

        if (!ownedCharaIds.Contains(c))
        {
            logger.LogError(
                "Request from DeviceAccount {id} contained not-owned character id {id}",
                deviceAccountId,
                id
            );
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateDragonKeyId(ulong keyId, string deviceAccountId)
    {
        // Empty slot
        if (keyId == 0)
            return true;

        IEnumerable<long> ownedDragonKeyIds = await this.unitRepository
            .GetAllDragonData(deviceAccountId)
            .Select(x => x.DragonKeyId)
            .ToListAsync();

        if (!ownedDragonKeyIds.Contains((long)keyId))
        {
            logger.LogError(
                "Request from DeviceAccount {id} contained not-owned dragon_key_id {key_id}",
                deviceAccountId,
                keyId
            );
            return false;
        }

        return true;
    }
}
