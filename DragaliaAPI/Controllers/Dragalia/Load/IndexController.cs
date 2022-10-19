using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia.Load;

[Route("load/index")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class IndexController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly ISessionService _sessionService;

    public IndexController(IApiRepository apiRepository, ISessionService sessionService)
    {
        _apiRepository = apiRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post([FromHeader(Name = "SID")] string sessionId)
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        DbPlayerUserData dbUserData = await _apiRepository
            .GetPlayerInfo(deviceAccountId)
            .SingleAsync();
        IEnumerable<DbPlayerCharaData> dbCharaData = await _apiRepository
            .GetCharaData(deviceAccountId)
            .ToListAsync();
        IEnumerable<DbPlayerDragonData> dbDragonData = await _apiRepository
            .GetDragonData(deviceAccountId)
            .ToListAsync();
        IEnumerable<DbParty> dbParties = await _apiRepository
            .GetParties(deviceAccountId)
            .ToListAsync();

        UserData userData = SavefileUserDataFactory.Create(dbUserData, new() { });
        IEnumerable<Chara> charas = dbCharaData.Select(CharaFactory.Create);
        IEnumerable<Dragon> dragons = dbDragonData.Select(DragonFactory.Create);
        IEnumerable<Party> parties = dbParties.Select(PartyFactory.Create);

        LoadIndexResponse response = new(new LoadIndexData(userData, charas, dragons, parties));

        return Ok(response);
    }
}
