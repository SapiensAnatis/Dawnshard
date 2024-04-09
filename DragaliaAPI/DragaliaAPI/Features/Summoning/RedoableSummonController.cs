using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.Summoning;

[Route("redoable_summon")]
public class RedoableSummonController(
    SummonService summonService,
    SummonOddsService summonOddsService,
    IStoryRepository storyRepository,
    IUnitRepository unitRepository,
    ITutorialService tutorialService,
    IUpdateDataService updateDataService,
    IDistributedCache cache
) : DragaliaControllerBase
{
    private const int PrologueStoryId = 1000100;
    private const int RerollTutorialStatus = 10152;

    private static class Schema
    {
        public static string SessionId_CachedSummonResult(string sessionId) =>
            $":cachedSummonResult:{sessionId}";
    }

    [HttpPost]
    [Route("get_data")]
    public async Task<DragaliaResult<RedoableSummonGetDataResponse>> GetData()
    {
        OddsRate normalOddsRate = await summonOddsService.GetNormalOddsRate(
            SummonConstants.RedoableSummonBannerId
        );
        OddsRate guaranteeRate = await summonOddsService.GetGuaranteeOddsRate(
            SummonConstants.RedoableSummonBannerId
        );

        return new RedoableSummonGetDataResponse()
        {
            UserRedoableSummonData = null,
            RedoableSummonOddsRateList = new RedoableSummonOddsRateList()
            {
                Normal = normalOddsRate,
                Guarantee = guaranteeRate
            }
        };
    }

    [HttpPost]
    [Route("pre_exec")]
    public async Task<DragaliaResult> PreExec([FromHeader(Name = "SID")] string sessionId)
    {
        IEnumerable<AtgenRedoableSummonResultUnitList> summonResult =
            await summonService.GenerateRedoableSummonResult();

        await cache.SetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId),
            JsonSerializer.Serialize(summonResult),
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            }
        );

        return this.Ok(
            new RedoableSummonPreExecResponse(new UserRedoableSummonData(true, summonResult))
        );
    }

    [HttpPost]
    [Route("fix_exec")]
    public async Task<DragaliaResult> FixExec(
        [FromHeader(Name = "SID")] string sessionId,
        CancellationToken cancellationToken
    )
    {
        string? cachedResultJson = await cache.GetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId),
            cancellationToken
        );

        if (string.IsNullOrEmpty(cachedResultJson))
            return this.BadRequest();

        List<AtgenRedoableSummonResultUnitList> cachedResult =
            JsonSerializer.Deserialize<List<AtgenRedoableSummonResultUnitList>>(cachedResultJson)
            ?? throw new JsonException("Null deserialization result!");

        await tutorialService.UpdateTutorialStatus(RerollTutorialStatus);

        DbPlayerStoryState prologueStory = await storyRepository.GetOrCreateStory(
            StoryTypes.Quest,
            PrologueStoryId
        );
        prologueStory.State = StoryState.Read;

        IEnumerable<(Charas id, bool isNew)> repositoryCharaOuput = await unitRepository.AddCharas(
            cachedResult.Where(x => x.EntityType == EntityTypes.Chara).Select(x => (Charas)x.Id)
        );

        IEnumerable<(Dragons Id, bool IsNew)> repositoryDragonOutput =
            await unitRepository.AddDragons(
                cachedResult
                    .Where(x => x.EntityType == EntityTypes.Dragon)
                    .Select(x => (Dragons)x.Id)
            );

        UpdateDataList updateData = await updateDataService.SaveChangesAsync(cancellationToken);

        IEnumerable<AtgenDuplicateEntityList> newCharas = repositoryCharaOuput
            .Where(x => x.isNew)
            .Select(x => new AtgenDuplicateEntityList()
            {
                EntityType = EntityTypes.Chara,
                EntityId = (int)x.id
            });
        IEnumerable<AtgenDuplicateEntityList> newDragons = repositoryDragonOutput
            .Where(x => x.IsNew)
            .Select(x => new AtgenDuplicateEntityList()
            {
                EntityType = EntityTypes.Dragon,
                EntityId = (int)x.Id
            });

        return this.Ok(
            new RedoableSummonFixExecResponse()
            {
                UserRedoableSummonData = new UserRedoableSummonData()
                {
                    IsFixedResult = true,
                    RedoableSummonResultUnitList = cachedResult
                },
                UpdateDataList = updateData,
                EntityResult = new EntityResult()
                {
                    NewGetEntityList = newCharas.Concat(newDragons)
                }
            }
        );
    }
}
