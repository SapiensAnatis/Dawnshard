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

    private static readonly ImmutableDictionary<AbilityCrestsRarities, double> BaseCrestQuantities =
        new Dictionary<AbilityCrestsRarities, double>()
        {
            // Determine drop rates of wyrmprints
            { AbilityCrestsRarities.SinisterDominion, 1 }, // Dominion prints
            { AbilityCrestsRarities.Common, 1 }, // 2 star
            { AbilityCrestsRarities.Uncommon, 1 }, // 3 star 
            { AbilityCrestsRarities.Rare, 1 }, // 4 star
            { AbilityCrestsRarities.VeryRare, 1 } // 5 star
        }.ToImmutableDictionary();

    private const int CrestPertubation = 3;

    public QuestEnemyService(IQuestDropService dropService, ILogger<QuestEnemyService> logger)
    {
        this.dropService = dropService;
        this.logger = logger;
    }

    public IEnumerable<AtgenEnemy> BuildQuestEnemyList(int questId, int areaNum)
    {
        List<AtgenEnemy> enemyList = this.GetEnemyList(questId, areaNum).ToList();
        IEnumerable<Materials> possibleMaterialDrops;
        IEnumerable<AbilityCrests> possibleCrestDrops;
        (possibleMaterialDrops, possibleCrestDrops) = this.dropService.GetDrops(questId);
        QuestData questData = MasterAsset.QuestData[questId];

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

        foreach (AtgenEnemy enemy in enemyList)
        {
            enemy.enemy_drop_list = GenerateEnemyDrop(
                enemy,
                possibleMaterialDrops,
                possibleCrestDrops,
                questMultiplier,
                difficultyMultiplier
            );
        }

        return enemyList;
    }

    private IEnumerable<EnemyDropList> GenerateEnemyDrop(
        AtgenEnemy enemy,
        IEnumerable<Materials> possibleMaterialDrops,
        IEnumerable<AbilityCrests> possibleCrestDrops,
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
                drop_list = GenerateMaterialDrops(
                    possibleMaterialDrops,
                    toughnessMultiplier * difficultyMultiplier,
                    dropMultiplier
                )
                /*
                crest_drops = GenerateCrestDrops(
                    possibleCrestDrops,
                    toughnessMultiplier * difficultyMultiplier,
                    dropMultiplier
                )

                drop_list.AddRange(crest_drops);
                */
            }
        };
    }

    private static IEnumerable<AtgenDropList> GenerateMaterialDrops(
        IEnumerable<Materials> possibleMaterialDrops,
        double baseMultiplier,
        QuestGroupMultiplier questMultiplier
    )
    {
        const double multiplierPower = 1.5;

        foreach (Materials material in possibleMaterialDrops)
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

    private static IEnumerable<AtgenDropList> GenerateCrestDrops(
        IEnumerable<AbilityCrests> possibleCrestDrops,
        double baseMultiplier,
        QuestGroupMultiplier questMultiplier
    )
    {
        const double multiplierPower = 1.5;

        foreach (AbilityCrests crest in possibleCrestDrops)
        {
            // Fairly confident this will be defined
            AbilityCrest crestData = MasterAsset.AbilityCrest[crest];

            int randomizedQuantity = CalculateQuantity(
                BaseCrestQuantities[crestData.Rarity],
                CrestPertubation,
                baseMultiplier,
                multiplierPower
            );

            
            randomizedQuantity = (int)(
                randomizedQuantity * questMultiplier.CrestMultiplier[crestData.Rarity]
            );

            if (randomizedQuantity <= 0)
                continue;

            yield return new()
            {
                id = (int)crest,
                quantity = randomizedQuantity,
                type = EntityTypes.Wyrmprint
            };

            
        }

        //return Enumerable.Empty<AtgenDropList>()
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
