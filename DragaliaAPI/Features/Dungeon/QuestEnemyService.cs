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
            { MaterialRarities.Uncommon, 0.3 },
            { MaterialRarities.Rare, 0.25 },
            { MaterialRarities.VeryRare, 0.1 } // Only used for Adamantite Ingot?
        }.ToImmutableDictionary();
    private const int MaterialPertubation = 5;

    public QuestEnemyService(IQuestDropService dropService, ILogger<QuestEnemyService> logger)
    {
        this.dropService = dropService;
        this.logger = logger;
    }

    public IEnumerable<AtgenEnemy> BuildEnemyList(int questId, int areaNum)
    {
        List<AtgenEnemy> enemyList = this.GetEnemyList(questId, areaNum).ToList();
        IEnumerable<Materials> possibleDrops = this.dropService.GetDrops(questId);

        if (!MasterAsset.QuestMultiplier.TryGetValue(questId, out QuestMultiplier? questMultiplier))
            questMultiplier = new(questId, 1, 1, 1);

        this.logger.LogDebug("Using quest multiplier: {@multiplier}", questMultiplier);

        const int manaToughnessPower = 3;
        const int coinToughnessPower = 4;
        const int toughnessLimit = 4;

        foreach (AtgenEnemy enemy in enemyList)
        {
            if (!MasterAsset.EnemyParam.TryGetValue(enemy.param_id, out EnemyParam? enemyParam))
            {
                this.logger.LogWarning("Failed to get EnemyParam for id {id}", enemy.param_id);
                continue;
            }

            int toughnessMultiplier = Math.Min((int)enemyParam.Tough + 1, toughnessLimit);

            List<EnemyDropList> dropListList =
                new()
                {
                    new()
                    {
                        mana = CalculateQuantity(
                            BaseManaQuantity,
                            ManaPertubation,
                            toughnessMultiplier * questMultiplier.ManaMultiplier,
                            manaToughnessPower
                        ),
                        coin = CalculateQuantity(
                            BaseRupieQuantity,
                            RupiePertubation,
                            toughnessMultiplier * questMultiplier.RupieMultiplier,
                            coinToughnessPower
                        ),
                        drop_list = GenerateMaterialDrops(
                            possibleDrops,
                            toughnessMultiplier * questMultiplier.MaterialMultiplier
                        )
                    }
                };

            enemy.enemy_drop_list = dropListList;
        }

        return enemyList;
    }

    private static IEnumerable<AtgenDropList> GenerateMaterialDrops(
        IEnumerable<Materials> possibleDrops,
        double multiplier
    )
    {
        const double materialToughnessPower = 3;

        foreach (Materials material in possibleDrops)
        {
            // Fairly confident this will be defined
            MaterialData materialData = MasterAsset.MaterialData[material];

            int randomizedQuantity = CalculateQuantity(
                BaseMaterialQuantities[materialData.MaterialRarity],
                MaterialPertubation,
                multiplier,
                materialToughnessPower
            );

            if (randomizedQuantity < 0)
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
