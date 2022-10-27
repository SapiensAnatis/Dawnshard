using System;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Services.Data;
using static DragaliaAPI.Models.Dragalia.Responses.Summon.SummonHistoryFactory;
using System.Collections.Immutable;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Models.Nintendo;
using Microsoft.AspNetCore.JsonPatch.Internal;

namespace DragaliaAPI.Services;

public class SummonService : ISummonService
{
    private readonly IUnitDataService _unitDataService;
    private readonly IDragonDataService _dragonDataService;

    private const float SSRSummonRateChara = 0.5f;
    private const float SSRSummonRateDragon = 0.8f;
    private const float SRSummonRateTotalNormal = 9.0f;
    private const float SRSummonRateTotalFeatured = 7.0f;
    private const float SRSummonRateTotal = SRSummonRateTotalNormal + SRSummonRateTotalFeatured;
    private const float RSummonRateChara = 4.0f;

    public SummonService(IUnitDataService unitDataService, IDragonDataService dragonDataService)
    {
        _unitDataService = unitDataService;
        _dragonDataService = dragonDataService;
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
                int rarity = _unitDataService.GetData(id).Rarity;
                resultList.Add(new((int)EntityTypes.Chara, (int)id, rarity));
            }
        }

        return resultList;
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
