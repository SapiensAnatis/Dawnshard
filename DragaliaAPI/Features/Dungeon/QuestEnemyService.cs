using System.Collections.Immutable;
using DragaliaAPI.Extensions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Features.Dungeon;

public class QuestEnemyService : IQuestEnemyService
{
    private readonly IQuestDropService dropService;
    private readonly ILogger<QuestEnemyService> logger;

    private const int BaseManaQuantity = 10;
    private const int ManaPertubation = 2;

    private const int BaseRupieQuantity = 50;
    private const int RupiePertubation = 5;

    private static readonly ImmutableDictionary<MaterialRarities, double> BaseMaterialQuantities =
        new Dictionary<MaterialRarities, double>()
        {
            // These numbers are totally arbitrary and just based on playtesting / what feels right
            // Bear in mind that common drops will be dropped by a lot more enemies on e.g. campaign maps
            { MaterialRarities.FacilityEvent, 2 },
            { MaterialRarities.Common, 0.1 },
            { MaterialRarities.Uncommon, 0.5 },
            { MaterialRarities.Rare, 0.1 },
            { MaterialRarities.VeryRare, 0.01 } // Only used for Adamantite Ingot?
        }.ToImmutableDictionary();
    private const int MaterialPertubation = 3;

    public QuestEnemyService(IQuestDropService dropService, ILogger<QuestEnemyService> logger)
    {
        this.dropService = dropService;
        this.logger = logger;
    }

    public IEnumerable<AtgenEnemy> BuildQuestEnemyList(int questId, int areaNum)
    {
        List<AtgenEnemy> enemyList = this.GetEnemyList(questId, areaNum).ToList();
        IEnumerable<Materials> possibleDrops = this.dropService.GetDrops(questId).ToList();
        var dropRarityDict = possibleDrops
            .ToLookup(x => MasterAsset.MaterialData[x].MaterialRarity, x => x)
            .ToDictionary(x => x.Key, x => x.ToList());

        QuestData questData = MasterAsset.QuestData[questId];

        int dropAmount = Random.Shared.Next(
            (int)Math.Floor(questData.PayStaminaSingle * 0.66f),
            questData.PayStaminaSingle
        );

        if (
            !MasterAsset.QuestGroupMultiplier.TryGetValue(
                questData.Gid,
                out QuestGroupMultiplier? questMultiplier
            )
        )
        {
            questMultiplier = new(questData.Gid);
        }

        this.logger.LogDebug("Using quest multiplier: {@multiplier}", questMultiplier);

        double difficultyCoeff = Math.Max(questData.Difficulty / 1000, Math.E);
        double difficultyMultiplier = Math.Log(difficultyCoeff);

        Dictionary<Materials, int> drops = new();

        for (int i = 0; i < dropAmount; i++)
        {
            int roll = Random.Shared.Next(100);

            List<Materials> pool = dropRarityDict.Values.First();

            if (
                roll > 75
                && dropRarityDict.TryGetValue(MaterialRarities.Uncommon, out List<Materials>? value)
            )
                pool = value;
            if (roll > 90 && dropRarityDict.TryGetValue(MaterialRarities.Rare, out value))
                pool = value;
            if (roll > 99 && dropRarityDict.TryGetValue(MaterialRarities.VeryRare, out value))
                pool = value;

            Materials drop = pool[Random.Shared.Next(pool.Count - 1)];
            if (drops.ContainsKey(drop))
                drops[drop]++;
            else
                drops[drop] = 1;
        }

        int remainingDropCount = drops.Sum(x => x.Value);
        int remainingEnemyCount = enemyList.Count;

        foreach (AtgenEnemy enemy in enemyList)
        {
            enemy.enemy_drop_list = GenerateEnemyDrop(
                enemy,
                possibleDrops,
                questMultiplier,
                difficultyMultiplier
            );
        }

        AssignDropsToEnemies(
            enemyList.Where(x => MasterAsset.EnemyParam[x.param_id].Tough == Toughness.Boss),
            true,
            0.5
        );
        AssignDropsToEnemies(
            enemyList.Where(x => MasterAsset.EnemyParam[x.param_id].Tough == Toughness.Menace),
            true,
            0.25
        );
        AssignDropsToEnemies(
            enemyList.Where(
                x =>
                    MasterAsset.EnemyParam[x.param_id].Tough
                        is not (Toughness.Boss or Toughness.Menace)
            )
        );

        return enemyList;

        void AssignDropsToEnemies(
            IEnumerable<AtgenEnemy> enemies,
            bool skipChance = false,
            double minPercentage = double.NaN
        )
        {
            foreach (AtgenEnemy enemy in enemies)
            {
                AssignDropsToEnemy(enemy, skipChance, minPercentage);
            }
        }

        void AssignDropsToEnemy(
            AtgenEnemy enemy,
            bool skipChance = false,
            double minPercentage = double.NaN
        )
        {
            if (remainingDropCount == 0)
                return;

            List<AtgenDropList> enemyDrops = new();

            if (
                skipChance
                || (Random.Shared.NextDouble() < 1d / remainingEnemyCount && remainingDropCount > 0)
            )
            {
                Dictionary<Materials, int> currentDrops = new();

                int minCount = Math.Max(
                    (int)
                        Math.Ceiling(
                            double.IsNaN(minPercentage) ? 1 : remainingDropCount * minPercentage
                        ),
                    1
                );
                int amountToDrop = Random.Shared.Next(minCount, remainingDropCount);
                for (int i = 0; i < amountToDrop; i++)
                {
                    Materials element = Random.Shared.NextElement(drops.ToList()).Key;
                    if (drops[element] == 1)
                        drops.Remove(element);
                    else
                        drops[element]--;

                    if (currentDrops.ContainsKey(element))
                        currentDrops[element]++;
                    else
                        currentDrops[element] = 1;

                    remainingDropCount--;
                }

                enemyDrops.AddRange(
                    currentDrops.Select(
                        x => new AtgenDropList(EntityTypes.Material, (int)x.Key, x.Value, 0)
                    )
                );
            }

            remainingEnemyCount--;

            if (remainingEnemyCount == 0 && drops.Count > 0)
            {
                enemyDrops.AddRange(
                    drops.Select(
                        x => new AtgenDropList(EntityTypes.Material, (int)x.Key, x.Value, 0)
                    )
                );
                drops.Clear();
            }

            enemy.enemy_drop_list.First().drop_list = enemyDrops;
        }
    }

    private IEnumerable<EnemyDropList> GenerateEnemyDrop(
        AtgenEnemy enemy,
        IEnumerable<Materials> possibleDrops,
        QuestGroupMultiplier dropMultiplier,
        double difficultyMultiplier
    )
    {
        const int manaPower = 3;
        const int coinPower = 4;
        const int toughnessClamp = 5;

        if (!MasterAsset.EnemyParam.TryGetValue(enemy.param_id, out EnemyParam? enemyParam))
        {
            this.logger.LogWarning("Failed to get EnemyParam for id {id}", enemy.param_id);
            return Enumerable.Empty<EnemyDropList>();
        }

        int toughnessMultiplier = Math.Min((int)enemyParam.Tough + 1, toughnessClamp) * 2;

        return new List<EnemyDropList>()
        {
            new()
            {
                mana = CalculateQuantity(
                    BaseManaQuantity,
                    ManaPertubation,
                    toughnessMultiplier * dropMultiplier.ManaMultiplier,
                    manaPower
                ),
                coin = CalculateQuantity(
                    BaseRupieQuantity,
                    RupiePertubation,
                    toughnessMultiplier * dropMultiplier.RupieMultiplier,
                    coinPower
                ),
                //drop_list = GenerateMaterialDrops(
                //    possibleDrops,
                //    toughnessMultiplier * difficultyMultiplier,
                //    dropMultiplier
                //)
            }
        };
    }

    private static IEnumerable<AtgenDropList> GenerateMaterialDrops(
        IEnumerable<Materials> possibleDrops,
        double baseMultiplier,
        QuestGroupMultiplier questMultiplier
    )
    {
        const double multiplierPower = 1.5;

        foreach (Materials material in possibleDrops)
        {
            // Fairly confident this will be defined
            MaterialData materialData = MasterAsset.MaterialData[material];

            int randomizedQuantity = CalculateQuantity(
                BaseMaterialQuantities[materialData.MaterialRarity],
                MaterialPertubation,
                baseMultiplier,
                multiplierPower
            );

            randomizedQuantity = (int)(
                randomizedQuantity * questMultiplier.MaterialMultiplier[materialData.MaterialRarity]
            );

            if (randomizedQuantity <= 0)
                continue;

            yield return new()
            {
                id = (int)material,
                quantity = randomizedQuantity,
                type = EntityTypes.Material
            };
        }
    }

    private static int CalculateQuantity(
        double baseQuantity,
        int pertubation,
        double multiplier,
        double multiplierPower
    ) =>
        (int)Math.Ceiling(baseQuantity * Math.Pow(multiplier, multiplierPower))
        + Random.Shared.Next(-pertubation, pertubation);

    private IEnumerable<AtgenEnemy> GetEnemyList(int questId, int areaNum)
    {
        QuestData questData = MasterAsset.QuestData[questId];
        if (!questData.AreaInfo.TryGetElementAt(areaNum, out AreaInfo? areaInfo))
        {
            this.logger.LogWarning(
                "Failed to get area number {no} from quest {quest}",
                areaNum,
                questId
            );
            return Enumerable.Empty<AtgenEnemy>();
        }

        string assetName = $"{areaInfo.ScenePath}/{areaInfo.AreaName}".ToLowerInvariant();

        if (!MasterAsset.QuestEnemies.TryGetValue(assetName, out QuestEnemies? enemies))
        {
            this.logger.LogWarning(
                "Unable to retrieve enemy list for quest {quest} with area {area}",
                questId,
                assetName
            );
            return Enumerable.Empty<AtgenEnemy>();
        }

        if (!enemies.Enemies.TryGetValue(questData.VariationType, out IEnumerable<int>? enemyList))
        {
            this.logger.LogWarning(
                "Unable to retrieve enemy list for variation type {type}",
                questData.VariationType
            );
            return Enumerable.Empty<AtgenEnemy>();
        }

        return enemyList.Select(
            (x, idx) =>
                new AtgenEnemy()
                {
                    enemy_idx = idx,
                    is_pop = true,
                    is_rare = false,
                    param_id = x,
                    enemy_drop_list = new List<EnemyDropList>() { }
                }
        );
    }
}
