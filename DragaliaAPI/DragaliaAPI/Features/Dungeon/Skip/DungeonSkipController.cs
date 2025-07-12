using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Skip;

[Route("dungeon_skip")]
public class DungeonSkipController(
    IDungeonRecordService dungeonRecordService,
    IDungeonRecordHelperService dungeonRecordHelperService,
    IPartyRepository partyRepository,
    IOddsInfoService oddsInfoService,
    IPaymentService paymentService,
    IRewardService rewardService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("start")]
    public async Task<DragaliaResult> Start(
        DungeonSkipStartRequest request,
        CancellationToken cancellationToken
    )
    {
        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: request.PlayCount)
        );

        IEnumerable<PartySettingList> party = (
            await partyRepository
                .GetPartyUnits(request.PartyNo)
                .AsNoTracking()
                .ToListAsync(cancellationToken)
        ).Select(PartyMapper.MapToPartySettingList);

        IngameResultData ingameData = await this.GetIngameResultData(
            request.QuestId,
            request.PlayCount,
            request.SupportViewerId,
            party
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartResponse()
            {
                IngameResultData = ingameData,
                UpdateDataList = updateDataList,
                EntityResult = entityResult,
            }
        );
    }

    [HttpPost("start_assign_unit")]
    public async Task<DragaliaResult> StartAssignUnit(
        DungeonSkipStartAssignUnitRequest request,
        CancellationToken cancellationToken
    )
    {
        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: request.PlayCount)
        );

        IngameResultData ingameData = await this.GetIngameResultData(
            request.QuestId,
            request.PlayCount,
            request.SupportViewerId,
            request.RequestPartySettingList
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartResponse()
            {
                IngameResultData = ingameData,
                UpdateDataList = updateDataList,
                EntityResult = entityResult,
            }
        );
    }

    [HttpPost("start_multiple_quest")]
    public async Task<DragaliaResult> StartMultipleQuest(
        DungeonSkipStartMultipleQuestRequest request,
        CancellationToken cancellationToken
    )
    {
        int totalSkipQuantity = request.RequestQuestMultipleList.Sum(x => x.PlayCount);

        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: totalSkipQuantity)
        );

        List<PartySettingList> party = (
            await partyRepository
                .GetPartyUnits(request.PartyNo)
                .AsNoTracking()
                .ToListAsync(cancellationToken)
        )
            .Select(PartyMapper.MapToPartySettingList)
            .ToList();

        List<IngameResultData> results = new(request.RequestQuestMultipleList.Count());

        foreach (AtgenRequestQuestMultipleList quest in request.RequestQuestMultipleList)
        {
            IngameResultData resultData = await this.GetIngameResultData(
                quest.QuestId,
                quest.PlayCount,
                request.SupportViewerId,
                party
            );
            results.Add(resultData);
        }

        IngameResultData combinedResults = CombineIngameResultData(results);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartMultipleQuestResponse()
            {
                IngameResultData = combinedResults,
                EntityResult = entityResult,
                UpdateDataList = updateDataList,
            }
        );
    }

    [HttpPost("start_multiple_quest_assign_unit")]
    public async Task<DragaliaResult> StartMultipleQuest(
        DungeonSkipStartMultipleQuestAssignUnitRequest request,
        CancellationToken cancellationToken
    )
    {
        // Unsure what calls this endpoint -- can't repeat daily bonus skip or do it with assigned team

        int totalSkipQuantity = request.RequestQuestMultipleList.Sum(x => x.PlayCount);

        await paymentService.ProcessPayment(
            new Entity(EntityTypes.SkipTicket, Quantity: totalSkipQuantity)
        );

        List<IngameResultData> results = new(request.RequestQuestMultipleList.Count());

        foreach (AtgenRequestQuestMultipleList quest in request.RequestQuestMultipleList)
        {
            IngameResultData resultData = await this.GetIngameResultData(
                quest.QuestId,
                quest.PlayCount,
                request.SupportViewerId,
                request.RequestPartySettingList
            );
            results.Add(resultData);
        }

        IngameResultData combinedResults = CombineIngameResultData(results);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        EntityResult entityResult = rewardService.GetEntityResult();

        return this.Ok(
            new DungeonSkipStartMultipleQuestResponse()
            {
                IngameResultData = combinedResults,
                EntityResult = entityResult,
                UpdateDataList = updateDataList,
            }
        );
    }

    private async Task<IngameResultData> GetIngameResultData(
        int questId,
        int playCount,
        ulong? supportViewerId,
        IEnumerable<PartySettingList> party
    )
    {
        if (!MasterAsset.QuestData.TryGetValue(questId, out QuestData? questData))
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Quest ID not recognised"
            );
        }

        DungeonSession session = new()
        {
            Party = party.Where(x => x.CharaId != 0),
            QuestData = questData,
            SupportViewerId = supportViewerId,
            IsHost = true,
            IsMulti = false,
            PlayCount = playCount,
        };

        Dictionary<int, IList<AtgenEnemy>> enemyList = new(questData.AreaInfo.Count);

        for (int areaIndex = 0; areaIndex < questData.AreaInfo.Count; areaIndex++)
        {
            List<AtgenEnemy> enemies = [];

            for (int i = 0; i < playCount; i++)
            {
                // TODO: chests and drop_obj
                OddsInfo oddsInfo = oddsInfoService.GetOddsInfo(questId, areaIndex);
                enemies.AddRange(oddsInfo.Enemy);
            }

            enemyList[areaIndex] = enemies;
        }

        session.EnemyList = enemyList;

        PlayRecord playRecord = new()
        {
            IsClear = true,
            Time = -1,
            TreasureRecord = session.EnemyList.Select(x => new AtgenTreasureRecord()
            {
                AreaIdx = x.Key,
                Enemy = x.Value.Select(_ => 1),
                DropObj = new List<int>(), // TODO
                EnemySmash =
                    new List<AtgenEnemySmash>() // TODO
                ,
            }),
        };

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            string.Empty,
            playRecord,
            session
        );

        (ingameResultData.HelperList, ingameResultData.HelperDetailList, int manaGained) =
            await dungeonRecordHelperService.ProcessHelperDataSolo(supportViewerId);

        ingameResultData.GrowRecord.TakeMana += manaGained;

        return ingameResultData;
    }

    private static IngameResultData CombineIngameResultData(
        IEnumerable<IngameResultData> ingameResultDatas
    ) => ingameResultDatas.Aggregate((acc, current) => acc.CombineWith(current));
}
