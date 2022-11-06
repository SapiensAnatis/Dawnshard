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
    private readonly ISessionService sessionService;
    private readonly ILogger<PartyController> logger;

    public PartyController(
        IPartyRepository partyRepository,
        IUnitRepository unitRepository,
        IUserDataRepository userDataRepository,
        ISessionService sessionService,
        ILogger<PartyController> logger
    )
    {
        this.partyRepository = partyRepository;
        this.unitRepository = unitRepository;
        this.userDataRepository = userDataRepository;
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
        IEnumerable<Charas> ownedCharaIds = await this.unitRepository
            .GetAllCharaData(deviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        IEnumerable<long> ownedDragonKeyIds = await this.unitRepository
            .GetAllDragonData(deviceAccountId)
            .Select(x => x.DragonKeyId)
            .ToListAsync();

        // Validate characters
        foreach (int id in requestParty.request_party_setting_list.Select(x => x.chara_id))
        {
            if (id == 0)
            {
                continue;
            }

            if (!Enum.IsDefined(typeof(Charas), id))
            {
                logger.LogError(
                    "Request from DeviceAccount {id} contained invalid character id {id}",
                    deviceAccountId,
                    id
                );
                return this.BadRequest();
            }

            Charas c = (Charas)Enum.ToObject(typeof(Charas), id);

            if (!ownedCharaIds.Contains(c))
            {
                logger.LogError(
                    "Request from DeviceAccount {id} contained not-owned character id {id}",
                    deviceAccountId,
                    id
                );
                return this.BadRequest();
            }
        }

        // Validate dragons
        foreach (
            long keyId in requestParty.request_party_setting_list.Select(
                x => (long)x.equip_dragon_key_id
            )
        )
        {
            if (!ownedDragonKeyIds.Contains(keyId) && keyId != 0)
            {
                logger.LogError(
                    "Request from DeviceAccount {id} contained invalid dragon_key_id {key_id}",
                    deviceAccountId,
                    keyId
                );
                return this.BadRequest();
            }
        }

        // Party is OK, update DB
        Party responseParty =
            new(
                requestParty.party_no,
                requestParty.party_name,
                requestParty.request_party_setting_list
            );
        DbParty dbEntry = PartyFactory.CreateDbEntry(deviceAccountId, responseParty);
        await partyRepository.SetParty(deviceAccountId, dbEntry);

        // Send response
        UpdateDataList updateDataList = new() { party_list = new List<Party>() { responseParty } };
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

        return this.Ok(new PartySetMainPartyNoResponse(new(request.main_party_no)));
    }
}
