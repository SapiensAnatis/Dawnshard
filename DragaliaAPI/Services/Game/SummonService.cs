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

    public List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(int numSummons)
    {
        return this.GenerateSummonResult(
            numSummons,
            10,
            0.0f /*,new(new(), new(), 6.0d, 80.0d) */,
            1020067
        );
    }

    public List<int> GetSummonData(int bannerId)
    {
    // Rufen Sie die Banner-spezifischen Daten mithilfe von MasterAsset.SummonData ab
    var bannerSummonData = MasterAsset.SummonData.Get(bannerId);

    // Erstellen Sie eine Liste für die ausgewählten Charaktere
    List<int> selectedCharaIds = new List<int>();

    // Hinzufügen der Charakter-IDs aus den PickupUnitId-Feldern in bannerSummonData
    selectedCharaIds.AddRange(new[]
    {
        (bannerSummonData.PickupUnitType1 == 1) ? bannerSummonData.PickupUnitId1 : 0,
        (bannerSummonData.PickupUnitType2 == 1) ? bannerSummonData.PickupUnitId2 : 0,
        (bannerSummonData.PickupUnitType3 == 1) ? bannerSummonData.PickupUnitId3 : 0,
        (bannerSummonData.PickupUnitType4 == 1) ? bannerSummonData.PickupUnitId4 : 0
    }.Where(id => id != 0));


    // Wählen Sie Charaktere mit 3 oder 4 Sternen aus CharaData aus
    var selectedCharacters = MasterAsset.CharaData
        .Enumerable
        .Where(chara => chara.Rarity == 3 || chara.Rarity == 4)
        .Select(chara => (int)chara.Id)
        .ToList();

    // Fügen Sie die IDs aus selectedCharacters zu selectedCharaIds hinzu
    selectedCharaIds.AddRange(selectedCharacters);

    // Geben Sie die IDs der ausgewählten Charaktere zurück
    return selectedCharaIds;
    }

    public List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(
        int numSummons,
        int summonsUntilNextPity,
        float pity,
        int bannerId
    )
    {
        List<AtgenRedoableSummonResultUnitList> resultList = new();

        List<int> selectedCharaIds = GetSummonData(bannerId);

        for (int i = 0; i < numSummons; i++)
        {
            bool isDragon = random.NextSingle() > 0.5;
            if (isDragon)
            {
                Dragons id = random.NextEnum<Dragons>();
                while (id == 0 || DragonConstants.unsummonableDragons.Contains(id))
                    id = random.NextEnum<Dragons>();

                int rarity = MasterAsset.DragonData.Get(id).Rarity;
                resultList.Add(new(EntityTypes.Dragon, (int)id, rarity));
            }
            else
            {
                Charas charaEnum;

                do
                {
                    int randomCharaId = selectedCharaIds[random.Next(selectedCharaIds.Count)];
                    charaEnum = (Charas)Enum.Parse(typeof(Charas), randomCharaId.ToString());
                } while (MasterAsset.CharaData[charaEnum].Availability == CharaAvailabilities.Story);

                int rarity = MasterAsset.CharaData
                             .Enumerable
                             .Where(x => x.Id == charaEnum)
                             .Select(x => x.Rarity)
                             .FirstOrDefault();

                resultList.Add(new(EntityTypes.Chara, (int)charaEnum, rarity));
            }
        }

        logger.LogDebug("Generated summon result: {@summonResult}", resultList);

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
