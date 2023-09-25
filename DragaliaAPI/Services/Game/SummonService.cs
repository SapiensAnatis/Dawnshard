using System.Diagnostics;
using AutoMapper;
using System.Linq;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Extensions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Services.Game;

public class SummonService : ISummonService
{
    private readonly IUnitRepository unitRepository;
    private readonly IMapper mapper;
    private readonly ILogger<SummonService> logger;

    private readonly Random random;

    private const float SSRSummonRateChara = 0.5f;
    private const float SSRSummonRateDragon = 0.8f;
    private const float SRSummonRateTotalNormal = 9.0f;
    private const float SRSummonRateTotalFeatured = 7.0f;
    private const float SRSummonRateTotal = SRSummonRateTotalNormal + SRSummonRateTotalFeatured;
    private const float RSummonRateChara = 4.0f;

    public SummonService(
        IUnitRepository unitRepository,
        IMapper mapper,
        ILogger<SummonService> logger
    )
    {
        this.unitRepository = unitRepository;
        this.mapper = mapper;
        this.random = Random.Shared;
        this.logger = logger;
    }

    /* public record BannerSummonInfo(
         Dictionary<EntityTypes, SummonableEntity> featured,
         Dictionary<EntityTypes, SummonableEntity> normal,
         double baseSsrRate,
         double baseRRate
     );*/

    //TODO
    /* public Dictionary<SummonableEntity, double> CalculateOdds(
         BannerSummonInfo bannerInfo,
         float pity
     )
     {
         Dictionary<SummonableEntity, double> pool = new Dictionary<SummonableEntity, double>();

         double realSsrRate = bannerInfo.baseSsrRate + pity;
         double realRRate = bannerInfo.baseRRate - pity;
         int countSrFeaturedRewards = bannerInfo.featured.Values.Where(x => x.rarity == 4).Count();
         int countSrRewards = bannerInfo.normal.Values.Where(x => x.rarity == 4).Count();
         foreach (
             Dictionary<EntityTypes, SummonableEntity> relPool in new List<
                 Dictionary<EntityTypes, SummonableEntity>
             >()
             {
                 bannerInfo.featured,
                 bannerInfo.normal
             }
         )
         {
             double ssrRateChara =
                 relPool == bannerInfo.featured
                     ? SSRSummonRateChara
                     : realSsrRate / bannerInfo.normal.Values.Where(x => x.rarity == 5).Count();
             double srRate =
                 relPool == bannerInfo.featured
                     ? SRSummonRateTotalFeatured / countSrFeaturedRewards
                     : realSsrRate / countSrFeaturedRewards;
             foreach (SummonableEntity summonableEntity in relPool.Values)
             {
                 double summonRate = 0d;
                 switch (summonableEntity.rarity)
                 {
                     case 5:
                         realSsrRate -= ssrRateChara;
                         break;
                     case 4:
                         summonRate -= ssrRateChara;
                         break;
                     case 3:

                         break;
                 }
                 pool.Add(summonableEntity, summonRate);
             }
         }
         return pool;
     }*/

    public List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(int numSummons, int bannerId)
    {
        return this.GenerateSummonResult(
            numSummons,
            10,
            0.0f /*,new(new(), new(), 6.0d, 80.0d) */,
            bannerId
        );
    }

    public Dictionary<int, Tuple<int, int>> GetSummonData(int bannerId)
    {
    
        Dictionary<int, Tuple<int, int>> selectedUnitDict = new Dictionary<int, Tuple<int, int>>();
        // Rufen Sie die Banner-spezifischen Daten mithilfe von MasterAsset.SummonData ab
        var bannerSummonData = MasterAsset.SummonData.Get(bannerId);

        if (bannerId == 1020203)
        {
            // Wenn bannerId gleich 1020203 ist, erstellen Sie die Liste aus der gesamten Charas-Enum-Liste.
            selectedUnitDict = Enum.GetValues(typeof(Charas))
                .Cast<Charas>()
                .Where(chara => (int)chara != 0)
                .ToDictionary(chara => (int)chara, chara => Tuple.Create(1, 5)); // Hier haben alle Charaktere den Typ 1
        }
        else
        {
            void AddToDict(int unitId, int unitType) {
            if (unitId != 0 && (unitType == 1 || unitType == 2) && !selectedUnitDict.ContainsKey(unitId)) {
                selectedUnitDict[unitId] = Tuple.Create(unitType, 5);
            }
        }

            AddToDict(bannerSummonData.PickupUnitId1, bannerSummonData.PickupUnitType1);
            AddToDict(bannerSummonData.PickupUnitId2, bannerSummonData.PickupUnitType2);
            AddToDict(bannerSummonData.PickupUnitId3, bannerSummonData.PickupUnitType3);
            AddToDict(bannerSummonData.PickupUnitId4, bannerSummonData.PickupUnitType4);
        }


        // Fügen Sie die Charakter-IDs aus CharaData zu selectedCharaDict hinzu
        foreach (var chara in MasterAsset.CharaData.Enumerable)
        {
            if (chara.Rarity == 3 || chara.Rarity == 4)
            {
                selectedUnitDict[(int)chara.Id] = Tuple.Create(1, chara.Rarity); // Hier setzen wir den Typ für Charaktere auf 1
            }
        }

        // Fügen Sie die Einheiten-IDs aus DragonData zu selectedUnitsDict hinzu

        foreach (var dragon in MasterAsset.DragonData.Enumerable)
        {
            string dragonIdString = dragon.Id.ToString();
            
            if (dragon.Rarity == 3 || dragon.Rarity == 4 || dragon.Rarity == 5 && dragon.IsPlayable == true)
            {
                selectedUnitDict[(int)dragon.Id] = Tuple.Create(2, dragon.Rarity); // Hier setzen wir den Typ für Drachen auf 2
            }
        }

        // Geben Sie die IDs der ausgewählten Charaktere zurück
        return selectedUnitDict;
    }

    public List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(
        int numSummons,
        int summonsUntilNextPity,
        float pity,
        int bannerId
    )
    {
        List<AtgenRedoableSummonResultUnitList> resultList = new();

        Dictionary<int, Tuple<int, int>> selectedUnitsDict = GetSummonData(bannerId);

        for (int i = 0; i < numSummons; i++)
        {
            bool isDragon = random.NextSingle() > 0.5;
            int rarity = (random.Next(2) == 0) ? 3 : 4;

            float fiveStarChance = 0.06f;

            if (random.NextSingle() <= fiveStarChance)
            {
                rarity = 5; // 5-star unit
            }

            if (isDragon)
            {
                List<int> dragonIds = selectedUnitsDict
                    .Where(entry => entry.Value.Item1 == 2 && entry.Value.Item2 == rarity && !DragonConstants.unsummonableDragons.Contains((Dragons)entry.Key))
                    .Select(entry => entry.Key)
                    .ToList();

                int randomDragonId = dragonIds[random.Next(dragonIds.Count)];
                Dragons id = (Dragons)Enum.Parse(typeof(Dragons), randomDragonId.ToString());

                int dragonRarity = MasterAsset.DragonData.Get(id).Rarity; // Verwenden Sie 'dragonRarity' hier
                resultList.Add(new(EntityTypes.Dragon, (int)id, dragonRarity));
            }
            else
            {
                List<int> charaIds = selectedUnitsDict
                    .Where(entry => entry.Value.Item1 == 1 && entry.Value.Item2 == rarity && MasterAsset.CharaData[(Charas)entry.Key].Availability != CharaAvailabilities.Story)
                    .Select(entry => entry.Key)
                    .ToList();

                int randomCharaId = charaIds[random.Next(charaIds.Count)];
                Charas charaEnum = (Charas)Enum.Parse(typeof(Charas), randomCharaId.ToString());

                int charaRarity = MasterAsset.CharaData
                        .Enumerable
                        .Where(x => x.Id == charaEnum)
                        .Select(x => x.Rarity)
                        .FirstOrDefault();

                resultList.Add(new(EntityTypes.Chara, (int)charaEnum, charaRarity));
            }
        }
        return resultList;
    }

    /// <summary>
    /// Populate a summon result with is_new and eldwater values.
    /// </summary>
    public List<AtgenResultUnitList> GenerateRewardList(
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    )
    {
        List<AtgenResultUnitList> newUnits = new();

        IEnumerable<Charas> ownedCharas = this.unitRepository.Charas.Select(x => x.CharaId);

        IEnumerable<Dragons> ownedDragons = this.unitRepository.Dragons.Select(x => x.DragonId);

        foreach (AtgenRedoableSummonResultUnitList reward in baseRewardList)
        {
            bool isNew = newUnits.All(x => x.id != reward.id);

            switch (reward.entity_type)
            {
                case EntityTypes.Chara:
                {
                    isNew |= ownedCharas.All(x => x != (Charas)reward.id);

                    AtgenResultUnitList toAdd =
                        new(
                            reward.entity_type,
                            reward.id,
                            reward.rarity,
                            isNew,
                            3,
                            isNew ? 0 : DewValueData.DupeSummon[reward.rarity]
                        );

                    newUnits.Add(toAdd);
                    break;
                }
                case EntityTypes.Dragon:
                {
                    isNew |= ownedDragons.All(x => x != (Dragons)reward.id);

                    AtgenResultUnitList toAdd =
                        new(reward.entity_type, reward.id, reward.rarity, isNew, 3, 0);

                    newUnits.Add(toAdd);
                    break;
                }
                default:
                    throw new UnreachableException(
                        "Invalid entity type for redoable summon result."
                    );
            }
        }

        return newUnits;
    }
}
