using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("party")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class PartyController : ControllerBase
{
    private readonly IApiRepository apiRepository;
    private readonly ISessionService sessionService;
    private readonly ILogger<PartyController> logger;

    public PartyController(
        IApiRepository apiRepository,
        ISessionService sessionService,
        ILogger<PartyController> logger
    )
    {
        this.apiRepository = apiRepository;
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
        return Ok(new PartyIndexResponse(new PartyIndexData(new(), new())));
    }

    [Route("set_party_setting")]
    public async Task<DragaliaResult> SetPartySetting(
        [FromHeader(Name = "SID")] string sessionId,
        SetPartySettingRequest requestParty
    )
    {
        string deviceAccountId = await sessionService.GetDeviceAccountId_SessionId(sessionId);

        // Validate the player owns all the entities they have requested to add
        // TODO: Weapon validation
        // TODO: Amulet validation
        // TODO: Talisman validation
        // TODO: Shared skill validation
        IEnumerable<Charas> ownedCharas = await apiRepository
            .GetCharaData(deviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        IEnumerable<long> ownedDragons = await apiRepository
            .GetDragonData(deviceAccountId)
            .Select(x => x.DragonKeyId)
            .ToListAsync();

        // Validate characters
        foreach (int id in requestParty.request_party_setting_list.Select(x => x.chara_id))
        {
            if (!Enum.IsDefined(typeof(Charas), id))
            {
                logger.LogError(
                    "Request from DeviceAccount {id} contained invalid character id {id}",
                    deviceAccountId,
                    id
                );
                return BadRequest();
            }
            Charas c = (Charas)Enum.ToObject(typeof(Charas), id);

            if (!ownedCharas.Contains(c))
            {
                logger.LogError(
                    "Request from DeviceAccount {id} contained not-owned character id {id}",
                    deviceAccountId,
                    id
                );
                return BadRequest();
            }
        }

        // Validate dragons
        foreach (
            long keyId in requestParty.request_party_setting_list.Select(
                x => (long)x.equip_dragon_key_id
            )
        )
        {
            if (!ownedDragons.Contains(keyId) && keyId != 0)
            {
                logger.LogError(
                    "Request from DeviceAccount {id} contained invalid dragon_key_id {key_id}",
                    deviceAccountId,
                    keyId
                );
                return BadRequest();
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
        await apiRepository.SetParty(deviceAccountId, dbEntry);

        // Send response
        UpdateDataList updateDataList = new() { party_list = new() { responseParty } };
        UpdateDataListResponse response = new(new(updateDataList));

        return Ok(response);
    }
}
