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
public class RedoableSummonController : DragaliaController
{
    private readonly ISummonService summonService;
    private readonly IQuestRepository questRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;
    private readonly IDistributedCache cache;

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
        IUpdateDataService updateDataService,
        IMapper mapper,
        IDistributedCache cache,
        ISessionService sessionService
    )
    {
        this.summonService = summonService;
        this.questRepository = questRepository;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
        this.cache = cache;
    }

    [HttpPost]
    [Route("get_data")]
    public DragaliaResult GetData()
    {
        RedoableSummonGetDataResponse response = new(RedoableSummonGetDataFactory.CreateData());
        return this.Ok(response);
    }

    [HttpPost]
    [Route("pre_exec")]
    public async Task<DragaliaResult> PreExec([FromHeader(Name = "SID")] string sessionId)
    {
        List<SimpleSummonReward> summonResult = summonService.GenerateSummonResult(50);
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

        await cache.SetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId),
            JsonSerializer.Serialize(summonResult)
        );

        RedoableSummonPreExecResponse response =
            new(RedoableSummonPreExecFactory.CreateData(summonResult));

        return this.Ok(response);
    }

    [HttpPost]
    [Route("fix_exec")]
    public async Task<DragaliaResult> FixExec([FromHeader(Name = "SID")] string sessionId)
    {
        string cachedResultJson = await cache.GetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId)
        );

        if (string.IsNullOrEmpty(cachedResultJson))
            return this.BadRequest();

        List<SimpleSummonReward> cachedResult =
            JsonSerializer.Deserialize<List<SimpleSummonReward>>(cachedResultJson)
            ?? throw new JsonException("Null deserialization result!");

        await userDataRepository.UpdateTutorialStatus(this.DeviceAccountId, 10152);
        await this.questRepository.UpdateQuestStory(this.DeviceAccountId, 1000100, 1); // Complete prologue story

        IEnumerable<DbPlayerCharaData> repositoryCharaOuput = await this.unitRepository.AddCharas(
            this.DeviceAccountId,
            cachedResult
                .Where(x => x.entity_type == (int)EntityTypes.Chara)
                .Select(x => (Charas)x.id)
        );

        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) = await this.unitRepository.AddDragons(
            this.DeviceAccountId,
            cachedResult
                .Where(x => x.entity_type == (int)EntityTypes.Dragon)
                .Select(x => (Dragons)x.id)
        );

        UpdateDataList updateData = this.updateDataService.GetUpdateDataList(this.DeviceAccountId);

        await this.unitRepository.SaveChangesAsync();

        return this.Ok(
            new RedoableSummonFixExecResponse(
                RedoableSummonFixExecFactory.CreateData(cachedResult, updateData)
            )
        );
    }
}
