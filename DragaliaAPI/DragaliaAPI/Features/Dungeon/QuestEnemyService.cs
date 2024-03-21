using DragaliaAPI.Extensions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Enemy;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;
using FluentRandomPicker;
using FluentRandomPicker.FluentInterfaces.General;

namespace DragaliaAPI.Features.Dungeon;

public class QuestEnemyService : IQuestEnemyService
{
    private readonly ILogger<QuestEnemyService> logger;

    public QuestEnemyService(ILogger<QuestEnemyService> logger)
    {
        this.logger = logger;
    }

    public IEnumerable<AtgenEnemy> BuildQuestEnemyList(int questId, int areaNum)
    {
        AtgenEnemy[] enemyList = this.GetEnemyList(questId, areaNum);

        if (!MasterAsset.QuestDrops.TryGetValue(questId, out QuestDropInfo? questDropInfo))
        {
            this.logger.LogWarning("Failed to get drop data for quest id {questId}", questId);
            return enemyList;
        }

        if (questDropInfo.Drops.Length == 0 || enemyList.Length == 0)
        {
            return enemyList;
        }

        int areaCount = MasterAsset.QuestData[questId].AreaInfo.Count();
        int totalQuantity = (int)Math.Round(questDropInfo.Drops.Sum(x => x.Quantity) / areaCount);

        int totalRupies = AddVariance(questDropInfo.Rupies) / areaCount;
        int rupieSlice = totalRupies / totalQuantity;

        int totalMana = AddVariance(questDropInfo.Mana) / areaCount;
        int manaSlice = totalMana / totalQuantity;

        IPick<DropEntity> dropPicker = GetDropPicker(questDropInfo);
        IPick<AtgenEnemy> enemyPicker = GetEnemyPicker(enemyList);

        for (int i = 0; i < totalQuantity; i++)
        {
            // TODO: Only give drops to boss enemies for boss quests with minions
            AtgenEnemy enemy = enemyPicker.PickOne();
            DropEntity drop = dropPicker.PickOne();

            if (enemy.EnemyDropList.Count == 0)
            {
                enemy.EnemyDropList.Add(
                    new EnemyDropList()
                    {
                        Coin = 0,
                        Mana = 0,
                        DropList = new List<AtgenDropList>()
                    }
                );
            }

            enemy.EnemyDropList[0].Coin += rupieSlice;
            enemy.EnemyDropList[0].Mana += manaSlice;

            enemy
                .EnemyDropList[0]
                .DropList.Add(
                    new AtgenDropList()
                    {
                        Id = drop.Id,
                        Type = drop.EntityType,
                        Quantity = 1
                    }
                );
        }

        // Accumulate quantities of the same drops
        foreach (AtgenEnemy enemy in enemyList)
        {
            if (enemy.EnemyDropList.Count == 0)
                continue;

            enemy.EnemyDropList[0].DropList = enemy
                .EnemyDropList[0]
                .DropList.GroupBy(x => new { id = x.Id, type = x.Type })
                .Select(group =>
                    group.Aggregate(
                        new AtgenDropList() { Id = group.Key.id, Type = group.Key.type },
                        (acc, current) =>
                        {
                            acc.Quantity += current.Quantity;
                            return acc;
                        }
                    )
                )
                .ToList();
        }

        return enemyList;
    }

    // Mercurial Gauntlet
    public IEnumerable<AtgenEnemy> BuildQuestWallEnemyList(int wallId, int wallLevel)
    {
        List<AtgenEnemy> enemyList = GetWallEnemyList(wallId, wallLevel).ToList();
        // should handle enemy drops but eh
        return enemyList;
    }

    private static IPick<AtgenEnemy> GetEnemyPicker(AtgenEnemy[] enemyList)
    {
        AtgenEnemy? boss = enemyList.FirstOrDefault(x =>
            MasterAsset.EnemyParam[x.ParamId].Tough >= Toughness.Boss
        );

        if (boss != null)
        {
            // Do not assign drops to minions
            enemyList = new[] { boss };
        }

        return GetPicker(
            enemyList,
            enemy =>
            {
                if (MasterAsset.EnemyParam.TryGetValue(enemy.ParamId, out EnemyParam? param))
                    return (int)param.Tough + 1;

                return 1;
            }
        );
    }

    private static IPick<DropEntity> GetDropPicker(QuestDropInfo questDropInfo) =>
        GetPicker(questDropInfo.Drops, entity => entity.Weight);

    private static IPick<T> GetPicker<T>(T[] elements, Func<T, int> weight) =>
        // Workaround for FluentRandomPicker throwing when passing in single-element collections
        elements.Length switch
        {
            1 => new SingleValuePicker<T>(elements[0]),
            > 1 => Out.Of().PrioritizedElements(elements).WithWeightSelector(weight),
            _ => throw new ArgumentException("Invalid value count", nameof(elements)),
        };

    private static int AddVariance(int value)
    {
        double range = value * 0.05;
        double variance = range * (Random.Shared.NextDouble() - 0.5);
        return (int)Math.Round(value + variance);
    }

    private AtgenEnemy[] GetEnemyList(int questId, int areaNum)
    {
        QuestData questData = MasterAsset.QuestData[questId];
        if (!questData.AreaInfo.TryGetElementAt(areaNum, out AreaInfo? areaInfo))
        {
            this.logger.LogWarning(
                "Failed to get area number {no} from quest {quest}",
                areaNum,
                questId
            );
            return Array.Empty<AtgenEnemy>();
        }

        string assetName = $"{areaInfo.ScenePath}/{areaInfo.AreaName}".ToLowerInvariant();

        if (!MasterAsset.QuestEnemies.TryGetValue(assetName, out QuestEnemies? enemies))
        {
            this.logger.LogWarning(
                "Unable to retrieve enemy list for quest {quest} with area {area}",
                questId,
                assetName
            );
            return Array.Empty<AtgenEnemy>();
        }

        if (!enemies.Enemies.TryGetValue(questData.VariationType, out IEnumerable<int>? enemyList))
        {
            this.logger.LogWarning(
                "Unable to retrieve enemy list for variation type {type}",
                questData.VariationType
            );
            return Array.Empty<AtgenEnemy>();
        }

        IEnumerable<EnemyParam> enemyParamList = enemyList.Select(x => MasterAsset.EnemyParam[x]);

        if (questData.EventKindType == EventKindType.Earn)
        {
            enemyParamList = enemyParamList.SelectMany(x =>
                DuplicateEarnEventEnemies(x, questData.VariationType)
            );
        }

        return enemyParamList
            .Where(x => x.Tough != Toughness.RareEnemy)
            .Select(
                (x, idx) =>
                    new AtgenEnemy()
                    {
                        EnemyIdx = idx,
                        IsPop = true,
                        IsRare = false,
                        ParamId = x.Id,
                        EnemyDropList = []
                    }
            )
            .ToArray();
    }

    // Mercurial Gauntlet
    private static List<AtgenEnemy> GetWallEnemyList(int wallId, int wallLevel)
    {
        QuestWallDetail questWallDetail = MasterAssetUtils.GetQuestWallDetail(wallId, wallLevel);
        return new List<AtgenEnemy>()
        {
            new AtgenEnemy()
            {
                EnemyIdx = 0,
                IsPop = true,
                IsRare = false,
                ParamId = questWallDetail.BossEnemyParamId,
                EnemyDropList = new List<EnemyDropList>()
            }
        };
    }

    private static IEnumerable<EnemyParam> DuplicateEarnEventEnemies(
        EnemyParam enemy,
        VariationTypes variationType
    )
    {
        yield return enemy;

        if (enemy.Tough == Toughness.Normal)
        {
            // Duplicate normal enemies by a factor of 3x
            yield return enemy;
            yield return enemy;

            // For Hard quests, duplicate 4x
            if (variationType == VariationTypes.Hard)
                yield return enemy;
        }

        // For Very Hard quests, duplicate normal enemies 4x, bosses 2x
        if (variationType == VariationTypes.VeryHard)
        {
            yield return enemy;
        }
    }
}
