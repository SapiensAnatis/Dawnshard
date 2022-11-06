using System.Text.Json;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
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
    private readonly IQuestRepository questRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IMapper mapper;
    private readonly IDistributedCache _cache;
    private readonly ISessionService _sessionService;

    private static class Schema
    {
        public static string SessionId_CachedSummonResult(string sessionId) =>
            $":{sessionId}:cachedSummonResult";
    }

    public RedoableSummonController(
        ISummonService summonService,
        IQuestRepository questRepository,
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IMapper mapper,
        IDistributedCache cache,
        ISessionService sessionService
    )
    {
        _summonService = summonService;
        this.questRepository = questRepository;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.mapper = mapper;
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

        await userDataRepository.UpdateTutorialStatus(deviceAccountId, 10152);
        await this.questRepository.UpdateQuestStory(deviceAccountId, 1000100, 1); // Complete prologue story

        IEnumerable<DbPlayerCharaData> repositoryCharaOuput = await this.unitRepository.AddCharas(
            deviceAccountId,
            cachedResult
                .Where(x => x.entity_type == (int)EntityTypes.Chara)
                .Select(x => (Charas)x.id)
        );

        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) repositoryDragonOutput = await this.unitRepository.AddDragons(
            deviceAccountId,
            cachedResult
                .Where(x => x.entity_type == (int)EntityTypes.Dragon)
                .Select(x => (Dragons)x.id)
        );

        UpdateDataList updateData = _summonService.GenerateUpdateData(
            repositoryCharaOuput,
            repositoryDragonOutput
        );

        updateData.user_data = this.mapper.Map<UserData>(
            await userDataRepository.GetUserData(deviceAccountId).SingleAsync()
        );

        return Ok(
            new RedoableSummonFixExecResponse(
                RedoableSummonFixExecFactory.CreateData(cachedResult, updateData)
            )
        );
    }
}
