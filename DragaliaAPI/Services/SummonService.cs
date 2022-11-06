using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Services;

public class SummonService : ISummonService
{
    private readonly ICharaDataService _charaDataService;
    private readonly IDragonDataService _dragonDataService;
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
        IMapper mapper
    )
    {
        _charaDataService = charaDataService;
        _dragonDataService = dragonDataService;
        this.mapper = mapper;
    }

    public record BannerSummonInfo(
        Dictionary<EntityTypes, SummonableEntity> featured,
        Dictionary<EntityTypes, SummonableEntity> normal,
        double baseSsrRate,
        double baseRRate
    );

    //TODO
    public Dictionary<SummonableEntity, double> CalculateOdds(
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
    }

    public List<SimpleSummonReward> GenerateSummonResult(int numSummons)
    {
        return GenerateSummonResult(numSummons, 10, 0.0f, new(new(), new(), 6.0d, 80.0d));
    }

    public List<SimpleSummonReward> GenerateSummonResult(
        int numSummons,
        int summonsUntilNextPity,
        float pity,
        BannerSummonInfo bannerInfo
    )
    {
        Random random = new((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        List<SimpleSummonReward> resultList = new();

        for (int i = 0; i < numSummons; i++)
        {
            bool isDragon = random.NextSingle() > 0.5;
            if (isDragon)
            {
                Dragons id = NextEnum<Dragons>(random);
                int rarity = _dragonDataService.GetData(id).Rarity;
                resultList.Add(new((int)EntityTypes.Dragon, (int)id, rarity));
            }
            else
            {
                Charas id = NextEnum<Charas>(random);
                int rarity = _charaDataService.GetData(id).Rarity;
                resultList.Add(new((int)EntityTypes.Chara, (int)id, rarity));
            }
        }

        return resultList;
    }

    public UpdateDataList GenerateUpdateData(
        IEnumerable<DbPlayerCharaData> repositoryCharaOutput,
        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) repositoryDragonOutput
    )
    {
        return new UpdateDataList()
        {
            chara_list = repositoryCharaOutput.Select(this.mapper.Map<Chara>),
            dragon_list = repositoryDragonOutput.newDragons.Select(this.mapper.Map<Dragon>),
            dragon_reliability_list = repositoryDragonOutput.newReliability.Select(
                this.mapper.Map<DragonReliability>
            )
        };
    }

    public IEnumerable<SummonReward> GenerateRewardList(
        IEnumerable<SimpleSummonReward> baseRewardList,
        IEnumerable<DbPlayerCharaData> repositoryCharaOutput,
        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) repositoryDragonOutput,
        bool giveDewPoint = true
    )
    {
        List<SummonReward> result = new();
        foreach (SimpleSummonReward reward in baseRewardList)
        {
            if (reward.entity_type == (int)EntityTypes.Chara)
            {
                SummonReward toAdd =
                    new(
                        reward.entity_type,
                        reward.id,
                        reward.rarity,
                        false,
                        giveDewPoint ? DewValueData.DupeSummon[reward.rarity] : 0
                    );

                if (
                    repositoryCharaOutput.Any(x => x.CharaId == (Charas)toAdd.id)
                    && !result.Any(x => x.id == toAdd.id)
                )
                {
                    toAdd = toAdd with { is_new = true, dew_point = 0 };
                }

                result.Add(toAdd);
            }
            else if (reward.entity_type == (int)EntityTypes.Dragon)
            {
                SummonReward toAdd = new(reward.entity_type, reward.id, reward.rarity, false, 0);

                if (
                    repositoryDragonOutput.newReliability.Any(x => x.DragonId == (Dragons)toAdd.id)
                    && !result.Any(x => x.id == toAdd.id)
                )
                {
                    toAdd = toAdd with { is_new = true };
                }

                result.Add(toAdd);
            }
        }

        return result;
    }

    private static T NextEnum<T>(Random random, T[] values) where T : struct, Enum
    {
        return (T)(
            values.GetValue(random.Next(values.Length))
            ?? throw new Exception("Invalid random value!")
        );
    }

    private static T NextEnum<T>(Random random) where T : struct, Enum
    {
        return NextEnum(random, Enum.GetValues<T>());
    }
}
