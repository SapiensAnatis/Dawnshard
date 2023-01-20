﻿using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("redoable_summon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class RedoableSummonController : DragaliaControllerBase
{
    private readonly ISummonService summonService;
    private readonly IQuestRepository questRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IDistributedCache cache;

    private static class Schema
    {
        public static string SessionId_CachedSummonResult(string sessionId) =>
            $":cachedSummonResult:{sessionId}";
    }

    private static readonly OddsRate OddsRate =
        new(
            new List<AtgenRarityList>()
            {
                new(5, "placeholder"),
                new(4, "placeholder"),
                new(3, "placeholder")
            },
            new List<AtgenRarityGroupList>()
            {
                new(false, 5, "placeholder", "placeholder", "placeholder", "placeholder")
            },
            new(
                new List<OddsUnitDetail>()
                {
                    new(
                        false,
                        5,
                        new List<AtgenUnitList>() { new((int)Charas.Addis, "placeholder") }
                    )
                },
                new List<OddsUnitDetail>()
                {
                    new(
                        false,
                        5,
                        new List<AtgenUnitList>() { new((int)Dragons.Agni, "placeholder") }
                    )
                },
                new List<OddsUnitDetail>()
                {
                    new(
                        false,
                        5,
                        new List<AtgenUnitList>()
                        {
                            new(40050001, "lol you can still summon prints")
                        }
                    )
                }
            )
        );

    private static readonly RedoableSummonGetDataData CachedData =
        new(null, new RedoableSummonOddsRateList(OddsRate, OddsRate));

    public RedoableSummonController(
        ISummonService summonService,
        IQuestRepository questRepository,
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IUpdateDataService updateDataService,
        IDistributedCache cache
    )
    {
        this.summonService = summonService;
        this.questRepository = questRepository;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.updateDataService = updateDataService;
        this.cache = cache;
    }

    [HttpPost]
    [Route("get_data")]
    public DragaliaResult GetData()
    {
        return this.Ok(CachedData);
    }

    [HttpPost]
    [Route("pre_exec")]
    public async Task<DragaliaResult> PreExec([FromHeader(Name = "SID")] string sessionId)
    {
        IEnumerable<AtgenRedoableSummonResultUnitList> summonResult = summonService
            .GenerateSummonResult(50)
            .Cast<AtgenRedoableSummonResultUnitList>();
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
            JsonSerializer.Serialize(summonResult),
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            }
        );

        return this.Ok(new RedoableSummonPreExecData(new UserRedoableSummonData(1, summonResult)));
    }

    [HttpPost]
    [Route("fix_exec")]
    public async Task<DragaliaResult> FixExec([FromHeader(Name = "SID")] string sessionId)
    {
        string? cachedResultJson = await cache.GetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId)
        );

        if (string.IsNullOrEmpty(cachedResultJson))
            return this.BadRequest();

        List<AtgenRedoableSummonResultUnitList> cachedResult =
            JsonSerializer.Deserialize<List<AtgenRedoableSummonResultUnitList>>(cachedResultJson)
            ?? throw new JsonException("Null deserialization result!");

        await userDataRepository.UpdateTutorialStatus(this.DeviceAccountId, 10152);
        await this.questRepository.UpdateQuestStory(this.DeviceAccountId, 1000100, 1); // Complete prologue story

        IEnumerable<(Charas id, bool isNew)> repositoryCharaOuput =
            await this.unitRepository.AddCharas(
                this.DeviceAccountId,
                cachedResult
                    .Where(x => x.entity_type == EntityTypes.Chara)
                    .Select(x => (Charas)x.id)
            );

        IEnumerable<(Dragons id, bool isNew)> repositoryDragonOutput =
            await this.unitRepository.AddDragons(
                this.DeviceAccountId,
                cachedResult
                    .Where(x => x.entity_type == EntityTypes.Dragon)
                    .Select(x => (Dragons)x.id)
            );

        UpdateDataList updateData = this.updateDataService.GetUpdateDataList(this.DeviceAccountId);

        await this.unitRepository.SaveChangesAsync();

        IEnumerable<AtgenDuplicateEntityList> newCharas = repositoryCharaOuput
            .Where(x => x.isNew)
            .Select(
                x =>
                    new AtgenDuplicateEntityList()
                    {
                        entity_type = EntityTypes.Chara,
                        entity_id = (int)x.id
                    }
            );
        IEnumerable<AtgenDuplicateEntityList> newDragons = repositoryDragonOutput
            .Where(x => x.isNew)
            .Select(
                x =>
                    new AtgenDuplicateEntityList()
                    {
                        entity_type = EntityTypes.Dragon,
                        entity_id = (int)x.id
                    }
            );

        return this.Ok(
            new RedoableSummonFixExecData()
            {
                user_redoable_summon_data = new UserRedoableSummonData()
                {
                    is_fixed_result = 1,
                    redoable_summon_result_unit_list = cachedResult
                },
                update_data_list = updateData,
                entity_result = new EntityResult()
                {
                    new_get_entity_list = newCharas.Concat(newDragons)
                }
            }
        );
    }
}
