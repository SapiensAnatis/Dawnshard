using System.Collections.Generic;
using System.Text.Json;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("redoable_summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class RedoableSummonController : ControllerBase
{
    private readonly ISummonService _summonService;
    private readonly IApiRepository _apiRepository;
    private readonly ISavefileWriteService _savefileWriteService;
    private readonly IDistributedCache _cache;
    private readonly ISessionService _sessionService;

    private static class Schema
    {
        public static string SessionId_CachedSummonResult(string sessionId) =>
            $":{sessionId}:cachedSummonResult";
    }

    public RedoableSummonController(
        ISummonService summonService,
        IApiRepository apiRepository,
        ISavefileWriteService savefileWriteService,
        IDistributedCache cache,
        ISessionService sessionService
    )
    {
        _summonService = summonService;
        _apiRepository = apiRepository;
        _savefileWriteService = savefileWriteService;
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
    public async Task<DragaliaResult> PreExec([FromHeader(Name = "SID")] string sessionId)
    {
        List<SimpleSummonReward> summonResult = _summonService.GenerateSummonResult(50);
        /*
        int testtype = 23;
        int testid = 0;
        List<SummonEntity> summonResult = new()
        {
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5),
            new(testtype, testid, 5)
        }; */

        await _cache.SetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId),
            JsonSerializer.Serialize(summonResult)
        );

        RedoableSummonPreExecResponse response =
            new(RedoableSummonPreExecFactory.CreateData(summonResult));

        return Ok(response);
    }

    [HttpPost]
    [Route("fix_exec")]
    public async Task<DragaliaResult> FixExec([FromHeader(Name = "SID")] string sessionId)
    {
        string cachedResultJson = await _cache.GetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId)
        );

        if (string.IsNullOrEmpty(cachedResultJson))
            return BadRequest();

        List<SimpleSummonReward> cachedResult =
            JsonSerializer.Deserialize<List<SimpleSummonReward>>(cachedResultJson)
            ?? throw new JsonException("Null deserialization result!");

        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

        await _apiRepository.UpdateTutorialStatus(deviceAccountId, 10152);

        UpdateDataList updateData = await _savefileWriteService.CommitSummonResult(
            cachedResult,
            deviceAccountId,
            giveDew: false
        );

        if (updateData.user_data is null)
            // Removes warning below
            throw new Exception("CommitSummonResult doesn't work properly");

        return Ok(
            new RedoableSummonFixExecResponse(
                RedoableSummonFixExecFactory.CreateData(cachedResult, updateData)
            )
        );
    }
}
