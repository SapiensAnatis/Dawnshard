using System.Text.Json;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Savefile;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Controllers.Dragalia.RedoableSummon;

[Route("redoable_summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class RedoableSummonController : ControllerBase
{
    private readonly ISummonService _summonService;
    private readonly IApiRepository _apiRepository;
    private readonly IDistributedCache _cache;
    private readonly ISessionService _sessionService;

    public RedoableSummonController(
        ISummonService summonService,
        IApiRepository apiRepository,
        IDistributedCache cache,
        ISessionService sessionService
    )
    {
        _summonService = summonService;
        _apiRepository = apiRepository;
        _cache = cache;
        _sessionService = sessionService;
    }

    [HttpPost]
    [Route("get_data")]
    public DragaliaResult GetData()
    {
        RedoableSummonGetDataResponse response = new(RedoableSummonGetDataFactory.CreateData());
        return Ok(response);
    }

    [HttpPost]
    [Route("pre_exec")]
    public DragaliaResult PreExec()
    {
        List<SummonEntity> summonResult = _summonService.GenerateSummonResult(50);
        RedoableSummonPreExecResponse response = new(new(new(0, summonResult), new(null)));
        return Ok(response);
    }

    [HttpPost]
    [Route("fix_exec")]
    [HttpGet]
    public async Task<DragaliaResult> PostExec([FromHeader(Name = "SID")] string sessionId)
    {
        try
        {
            string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

            string summonCacheKey = $"key";
            List<SummonEntity> cachedSummonResults = JsonSerializer.Deserialize<List<SummonEntity>>(
                await _cache.GetStringAsync(summonCacheKey)
            )!;
            Tuple<
                IEnumerable<Entity>,
                IEnumerable<Entity>,
                IEnumerable<Entity>
            > orderedPostCommitLists = await _apiRepository.commitSummonResults(
                deviceAccountId,
                cachedSummonResults
            );
            List<Entity> convertedSummonResults = orderedPostCommitLists.Item1.ToList();
            List<Entity> newSummonResults = orderedPostCommitLists.Item2.ToList();
            DbSavefileUserData dbUserData = await _apiRepository
                .GetPlayerInfo(deviceAccountId)
                .SingleAsync();
            SavefileUserData userData = SavefileUserDataFactory.Create(dbUserData, new() { });

            RedoableSummonFixExecResponse response =
                new(
                    RedoableSummonFixExecFactory.CreateData(
                        cachedSummonResults,
                        convertedSummonResults,
                        newSummonResults,
                        userData
                    )
                );
            return Ok(response);
        }
        catch (Exception)
        {
            return Ok(new ServerErrorResponse());
        }
    }
}
