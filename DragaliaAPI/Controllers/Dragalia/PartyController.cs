using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("party")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PartyController : ControllerBase
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
    [Route("index")]
    public DragaliaResult Index()
    {
        return this.Ok(new PartyIndexResponse(new PartyIndexData(new(), new())));
    }

    [Route("set_party_setting")]
    public async Task<DragaliaResult> SetPartySetting(
        [FromHeader(Name = "SID")] string sessionId,
        PartySetPartySettingRequest requestParty
    )
    {
        string deviceAccountId = await sessionService.GetDeviceAccountId_SessionId(sessionId);

        // Validate the player owns all the entities they have requested to add
        // TODO: Weapon validation
        // TODO: Amulet validation
        // TODO: Talisman validation
        // TODO: Shared skill validation

        foreach (PartyUnit partyUnit in requestParty.request_party_setting_list)
        {
            if (
                !await this.ValidateCharacterId(partyUnit.chara_id, deviceAccountId)
                || !await this.ValidateDragonKeyId(partyUnit.equip_dragon_key_id, deviceAccountId)
            )
            {
                return this.BadRequest();
            }
        }

        DbParty dbEntry = mapper.Map<DbParty>(
            new Party(
                requestParty.party_no,
                requestParty.party_name,
                requestParty.request_party_setting_list
            )
        );
        await partyRepository.SetParty(deviceAccountId, dbEntry);
        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(deviceAccountId);
        await partyRepository.SaveChangesAsync();

        UpdateDataListResponse response = new(new(updateDataList));

        return this.Ok(response);
    }

    [Route("set_main_party_no")]
    public async Task<DragaliaResult> SetMainPartyNo(
        [FromHeader(Name = "SID")] string sessionId,
        PartySetMainPartyNoRequest request
    )
    {
        string deviceAccountId = await sessionService.GetDeviceAccountId_SessionId(sessionId);

        await this.userDataRepository.SetMainPartyNo(deviceAccountId, request.main_party_no);
        await this.userDataRepository.SaveChangesAsync();

        return this.Ok(new PartySetMainPartyNoResponse(new(request.main_party_no)));
    }

    private async Task<bool> ValidateCharacterId(int id, string deviceAccountId)
    {
        // Empty slot
        if (id == 0)
        {
            return true;
        }

        IEnumerable<Charas> ownedCharaIds = await this.unitRepository
            .GetAllCharaData(deviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        if (!Enum.IsDefined(typeof(Charas), id))
        {
            logger.LogError(
                "Request from DeviceAccount {id} contained invalid character id {id}",
                deviceAccountId,
                id
            );
            return false;
        }

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
        {
            return true;
        }

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
