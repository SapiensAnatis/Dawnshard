using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Features.Summoning;

[Route("redoable_summon")]
public class RedoableSummonController(
    SummonService summonService,
    SummonOddsService summonOddsService,
    UnitService unitService,
    IStoryRepository storyRepository,
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
    public DragaliaResult<RedoableSummonGetDataResponse> GetData()
    {
        // The reroll banner does not have a pity mechanic.
        const int summonCountSinceLastFiveStar = 0;

        OddsRate normalOddsRate = summonOddsService.GetNormalOddsRate(
            SummonConstants.RedoableSummonBannerId,
            summonCountSinceLastFiveStar
        );
        OddsRate? guaranteeRate = summonOddsService.GetGuaranteeOddsRate(
            SummonConstants.RedoableSummonBannerId,
            summonCountSinceLastFiveStar
        );

        return new RedoableSummonGetDataResponse()
        {
            UserRedoableSummonData = null,
            RedoableSummonOddsRateList = new RedoableSummonOddsRateList()
            {
                Normal = normalOddsRate,
                Guarantee = guaranteeRate,
            },
        };
    }

    [HttpPost]
    [Route("pre_exec")]
    public async Task<DragaliaResult> PreExec(
        [FromHeader(Name = Headers.SessionId)] string sessionId
    )
    {
        IEnumerable<AtgenRedoableSummonResultUnitList> summonResult =
            await summonService.GenerateRedoableSummonResult();

        await cache.SetStringAsync(
            Schema.SessionId_CachedSummonResult(sessionId),
            JsonSerializer.Serialize(summonResult),
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
            }
        );

        return this.Ok(
            new RedoableSummonPreExecResponse(new UserRedoableSummonData(true, summonResult))
        );
    }

    [HttpPost]
    [Route("fix_exec")]
    public async Task<DragaliaResult> FixExec(
        [FromHeader(Name = Headers.SessionId)] string sessionId,
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

        List<DragonId> dragonList = cachedResult
            .Where(x => x.EntityType == EntityTypes.Dragon)
            .Select(x => (DragonId)x.Id)
            .ToList();

        List<Charas> charaList = cachedResult
            .Where(x => x.EntityType == EntityTypes.Chara)
            .Select(x => (Charas)x.Id)
            .ToList();

        IEnumerable<(Charas id, bool isNew)> repositoryCharaOuput = await unitService.AddCharas(
            charaList
        );

        IEnumerable<(DragonId Id, bool IsNew)> repositoryDragonOutput =
            await unitService.AddDragons(dragonList);

        UpdateDataList updateData = await updateDataService.SaveChangesAsync(cancellationToken);

        IEnumerable<AtgenDuplicateEntityList> newCharas = repositoryCharaOuput
            .Where(x => x.isNew)
            .Select(x => new AtgenDuplicateEntityList()
            {
                EntityType = EntityTypes.Chara,
                EntityId = (int)x.id,
            });
        IEnumerable<AtgenDuplicateEntityList> newDragons = repositoryDragonOutput
            .Where(x => x.IsNew)
            .Select(x => new AtgenDuplicateEntityList()
            {
                EntityType = EntityTypes.Dragon,
                EntityId = (int)x.Id,
            });

        return this.Ok(
            new RedoableSummonFixExecResponse()
            {
                UserRedoableSummonData = new UserRedoableSummonData()
                {
                    IsFixedResult = true,
                    RedoableSummonResultUnitList = cachedResult,
                },
                UpdateDataList = updateData,
                EntityResult = new EntityResult()
                {
                    NewGetEntityList = newCharas.Concat(newDragons),
                },
            }
        );
    }
}
