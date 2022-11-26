using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Services;

public class SummonService : ISummonService
{
    private readonly ICharaDataService _charaDataService;
    private readonly IDragonDataService _dragonDataService;
    private readonly IUnitRepository unitRepository;
    private readonly IMapper mapper;

    private const float SSRSummonRateChara = 0.5f;
    private const float SSRSummonRateDragon = 0.8f;
    private const float SRSummonRateTotalNormal = 9.0f;
    private const float SRSummonRateTotalFeatured = 7.0f;
    private const float SRSummonRateTotal = SRSummonRateTotalNormal + SRSummonRateTotalFeatured;
    private const float RSummonRateChara = 4.0f;

    public SummonService(
        ICharaDataService charaDataService,
        IDragonDataService dragonDataService,
        IUnitRepository unitRepository,
        IMapper mapper
    )
    {
        _charaDataService = charaDataService;
        _dragonDataService = dragonDataService;
        this.unitRepository = unitRepository;
        this.mapper = mapper;
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
            0.0f /*,new(new(), new(), 6.0d, 80.0d) */
        );
    }

    public List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(
        int numSummons,
        int summonsUntilNextPity,
        float pity /*,
        BannerSummonInfo bannerInfo */
    )
    {
        Random random = new((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        List<AtgenRedoableSummonResultUnitList> resultList = new();

        for (int i = 0; i < numSummons; i++)
        {
            bool isDragon = random.NextSingle() > 0.5;
            if (isDragon)
            {
                Dragons id = NextEnum<Dragons>(random);
                while (id == 0)
                    id = NextEnum<Dragons>(random);

                int rarity = _dragonDataService.GetData(id).Rarity;
                resultList.Add(new((int)EntityTypes.Dragon, (int)id, rarity));
            }
            else
            {
                Charas id = NextEnum<Charas>(random);
                while (id == 0)
                    id = NextEnum<Charas>(random);

                int rarity = _charaDataService.GetData(id).Rarity;
                resultList.Add(new((int)EntityTypes.Chara, (int)id, rarity));
            }
        }

        return resultList;
    }

    /// <summary>
    /// Populate a summon result with is_new and eldwater values.
    /// </summary>
    public List<AtgenResultUnitList> GenerateRewardList(
        string deviceAccountId,
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    )
    {
        List<AtgenResultUnitList> result = new();
        IEnumerable<Charas> ownedCharas = this.unitRepository
            .GetAllCharaData(deviceAccountId)
            .Select(x => x.CharaId);
        IEnumerable<Dragons> ownedDragons = this.unitRepository
            .GetAllDragonData(deviceAccountId)
            .Select(x => x.DragonId);

        foreach (AtgenRedoableSummonResultUnitList reward in baseRewardList)
        {
            if (reward.entity_type == (int)EntityTypes.Chara)
            {
                AtgenResultUnitList toAdd =
                    new(
                        reward.entity_type,
                        reward.id,
                        reward.rarity,
                        false,
                        3,
                        DewValueData.DupeSummon[reward.rarity]
                    );

                if (
                    ownedCharas.Any(x => x == (Charas)toAdd.id)
                    && !result.Any(x => x.id == toAdd.id)
                )
                {
                    toAdd.is_new = true;
                    toAdd.dew_point = 0;
                }

                result.Add(toAdd);
            }
            else if (reward.entity_type == (int)EntityTypes.Dragon)
            {
                AtgenResultUnitList toAdd =
                    new(reward.entity_type, reward.id, reward.rarity, false, 3, 0);

                if (
                    ownedDragons.Any(x => x == (Dragons)toAdd.id)
                    && !result.Any(x => x.id == toAdd.id)
                )
                {
                    toAdd.is_new = true;
                }

                result.Add(toAdd);
            }
        }

        return result;
    }

    private static T NextEnum<T>(Random random) where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();

        return (T)(
            values.GetValue(random.Next(values.Length))
            ?? throw new Exception("Invalid random value!")
        );
    }
}
