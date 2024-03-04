using System.Diagnostics;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Summoning;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

public class SummonOddsService(IOptionsMonitor<SummonBannerOptions> optionsMonitor)
{
    private readonly SummonBannerOptions summonOptions = optionsMonitor.CurrentValue;

    public OddsRate? GetNormalOddsRate(int bannerId)
    {
        Banner? banner = this.summonOptions.Banners.FirstOrDefault(x => x.Id == bannerId);

        if (banner is null)
            return null;

        // TODO: Factor in pity rate
        RarityList fiveStarList = new(5, [], []);
        RarityList fourStarList = new(4, [], []);
        RarityList threeStarList = new(3, [], []);

        foreach (UnitRate rate in GetUnitRarities(banner))
        {
            AtgenUnitList unitList = rate.ToAtgenUnitList();
            RarityList list = rate.Rarity switch
            {
                5 => fiveStarList,
                4 => fourStarList,
                3 => threeStarList,
                _ => throw new UnreachableException($"Invalid rarity: {rate.Rarity}")
            };

            if (rate.EntityType == EntityTypes.Chara)
            {
                list.CharaList.Add(unitList);
                list.CharaRate += rate.Rate;
            }
            else if (rate.EntityType == EntityTypes.Dragon)
            {
                list.DragonList.Add(unitList);
                list.DragonRate += rate.Rate;
            }
            else
            {
                throw new UnreachableException(
                    $"Invalid rarity entity type ({rate.EntityType}) or rarity ({rate.Rarity})"
                );
            }
        }

        return new OddsRate()
        {
            RarityList =
            [
                fiveStarList.ToRarityList(),
                fourStarList.ToRarityList(),
                threeStarList.ToRarityList(),
            ],
            RarityGroupList =
            [
                fiveStarList.ToRarityGroupList(),
                fourStarList.ToRarityGroupList(),
                threeStarList.ToRarityGroupList()
            ],
            Unit = new()
            {
                CharaOddsList =
                [
                    fiveStarList.ToAdvOddsUnitDetail(),
                    fourStarList.ToAdvOddsUnitDetail(),
                    threeStarList.ToAdvOddsUnitDetail()
                ],
                DragonOddsList =
                [
                    fiveStarList.ToDragonOddsUnitDetail(),
                    fourStarList.ToDragonOddsUnitDetail(),
                    threeStarList.ToDragonOddsUnitDetail(),
                ]
            }
        };
    }

    private static IEnumerable<UnitRate> GetUnitRarities(Banner banner)
    {
        List<CharaData> featuredCharaData = banner
            .FeaturedAdventurers.Select(x => MasterAsset.CharaData[x])
            .ToList();

        List<DragonData> featuredDragonData = banner
            .FeaturedDragons.Select(x => MasterAsset.DragonData[x])
            .ToList();

        BaseRateData rateData = GetBaseRates(banner);

        int totalFeaturedFourStar =
            featuredCharaData.Count(x => x.Rarity == 4)
            + featuredDragonData.Count(x => x.Rarity == 4);

        foreach (CharaData data in featuredCharaData)
        {
            decimal rate;
            switch (data.Rarity)
            {
                case 5:
                    rate = 0.005m;
                    rateData.FiveStarAdvRate -= rate;
                    break;
                case 4:
                    rate = 0.07m / totalFeaturedFourStar;
                    rateData.FourStarAdvRate -= rate;
                    break;
                case 3:
                    rate = 0.04m;
                    rateData.ThreeStarAdvRate -= rate;
                    break;
                default:
                    throw new UnreachableException(
                        $"Invalid rarity {data.Rarity} for character {data.Id}"
                    );
            }

            yield return new UnitRate(data.Id, rate);
        }

        foreach (DragonData data in featuredDragonData)
        {
            decimal rate;
            switch (data.Rarity)
            {
                case 5:
                    rate = 0.008m;
                    rateData.FiveStarDragonRate -= rate;
                    break;
                case 4:
                    rate = 0.07m / totalFeaturedFourStar;
                    rateData.FourStarDragonRate -= rate;
                    break;
                case 3:
                    rate = 0.04m;
                    rateData.ThreeStarDragonRate -= rate;
                    break;
                default:
                    throw new UnreachableException(
                        $"Invalid rarity {data.Rarity} for character {data.Id}"
                    );
            }

            yield return new UnitRate(data.Id, rate);
        }

        List<CharaData> charaPool = Enum.GetValues<Charas>()
            .Where(x => IsCharaInBannerRegularPool(x, banner))
            .Select(x => MasterAsset.CharaData[x])
            .ToList();
        List<DragonData> dragonPool = Enum.GetValues<Dragons>()
            .Where(x => IsDragonInBannerRegularPool(x, banner))
            .Select(x => MasterAsset.DragonData[x])
            .ToList();

        PoolSizeData charaPoolData = GetPoolSizeByRarity(charaPool);
        PoolSizeData dragonPoolData = GetPoolSizeByRarity(dragonPool);

        foreach (CharaData chara in charaPool)
        {
            decimal rate = chara.Rarity switch
            {
                5 => rateData.FiveStarAdvRate / charaPoolData.FiveStarPoolSize,
                4 => rateData.FourStarAdvRate / charaPoolData.FourStarPoolSize,
                3 => rateData.ThreeStarAdvRate / charaPoolData.ThreeStarPoolSize,
                _
                    => throw new UnreachableException(
                        $"Invalid rarity {chara.Rarity} for character {chara.Id}"
                    )
            };

            yield return new UnitRate(chara.Id, rate);
        }

        foreach (DragonData dragon in dragonPool)
        {
            decimal rate = dragon.Rarity switch
            {
                5 => rateData.FiveStarDragonRate / dragonPoolData.FiveStarPoolSize,
                4 => rateData.FourStarDragonRate / dragonPoolData.FourStarPoolSize,
                3 => rateData.ThreeStarDragonRate / dragonPoolData.ThreeStarPoolSize,
                _
                    => throw new UnreachableException(
                        $"Invalid rarity {dragon.Rarity} for dragon {dragon.Id}"
                    )
            };

            yield return new UnitRate(dragon.Id, rate);
        }
    }

    private static bool IsCharaInBannerRegularPool(Charas chara, Banner banner)
    {
        if (chara == Charas.Empty)
            return false; // Hack to stop masterasset lookup fails

        if (banner.FeaturedAdventurers.Contains(chara))
            return false; // They are in the featured pool instead.

        UnitAvailability availability = chara.GetAvailability();

        return availability switch
        {
            UnitAvailability.Permanent => true,
            UnitAvailability.Gala => banner.IsGala,
            UnitAvailability.Limited => banner.LimitedAdventurers.Contains(chara),
            _ => false
        };
    }

    private static bool IsDragonInBannerRegularPool(Dragons dragon, Banner banner)
    {
        if (dragon == Dragons.Empty)
            return false; // Hack to stop masterasset lookup fails

        if (banner.FeaturedDragons.Contains(dragon))
            return false; // They are in the featured pool instead.

        UnitAvailability availability = dragon.GetAvailability();

        return availability switch
        {
            UnitAvailability.Permanent => true,
            UnitAvailability.Gala => banner.IsGala,
            UnitAvailability.Limited => banner.LimitedDragons.Contains(dragon),
            _ => false
        };
    }

    private static BaseRateData GetBaseRates(Banner banner)
    {
        decimal fiveStarRate = banner.IsGala ? 0.06m : 0.04m;

        decimal fourStarAdvRate;
        decimal fourStarDragonRate;

        bool anyFourStarFeatured =
            banner.FeaturedAdventurers.Any(x => MasterAsset.CharaData[x].Rarity == 4)
            || banner.FeaturedDragons.Any(x => MasterAsset.DragonData[x].Rarity == 4);

        if (anyFourStarFeatured)
        {
            fourStarAdvRate = 0.045m;
            fourStarDragonRate = 0.045m;
        }
        else
        {
            fourStarAdvRate = 0.0855m;
            fourStarDragonRate = 0.0745m;
        }

        decimal threeStarAdvRate = banner.IsGala ? 0.47m : 0.48m;
        decimal threeStarDragonRate = banner.IsGala ? 0.31m : 0.32m;

        return new()
        {
            FiveStarAdvRate = fiveStarRate / 2,
            FiveStarDragonRate = fiveStarRate / 2,
            FourStarAdvRate = fourStarAdvRate,
            FourStarDragonRate = fourStarDragonRate,
            ThreeStarAdvRate = threeStarAdvRate,
            ThreeStarDragonRate = threeStarDragonRate
        };
    }

    private static PoolSizeData GetPoolSizeByRarity<TUnitData>(IEnumerable<TUnitData> pool)
        where TUnitData : IUnitData
    {
        int fiveStarPoolSize = 0;
        int fourStarPoolSize = 0;
        int threeStarPoolSize = 0;

        foreach (TUnitData data in pool)
        {
            switch (data.Rarity)
            {
                case 5:
                    fiveStarPoolSize++;
                    break;
                case 4:
                    fourStarPoolSize++;
                    break;
                case 3:
                    threeStarPoolSize++;
                    break;
            }
        }

        return new(fiveStarPoolSize, fourStarPoolSize, threeStarPoolSize);
    }

    private record struct BaseRateData(
        decimal FiveStarAdvRate,
        decimal FiveStarDragonRate,
        decimal FourStarAdvRate,
        decimal FourStarDragonRate,
        decimal ThreeStarAdvRate,
        decimal ThreeStarDragonRate
    );

    private readonly record struct PoolSizeData(
        int FiveStarPoolSize,
        int FourStarPoolSize,
        int ThreeStarPoolSize
    );

    private record RarityList(
        int Rarity,
        List<AtgenUnitList> DragonList,
        List<AtgenUnitList> CharaList
    )
    {
        public decimal CharaRate { get; set; }

        public decimal DragonRate { get; set; }

        public AtgenRarityList ToRarityList() =>
            new() { Rarity = Rarity, TotalRate = (CharaRate + DragonRate).ToPercentageString() };

        public AtgenRarityGroupList ToRarityGroupList() =>
            new()
            {
                Rarity = Rarity,
                TotalRate = (CharaRate + DragonRate).ToPercentageString(),
                DragonRate = DragonRate.ToPercentageString(),
                CharaRate = CharaRate.ToPercentageString()
            };

        public OddsUnitDetail ToAdvOddsUnitDetail() =>
            new() { Rarity = Rarity, UnitList = this.CharaList };

        public OddsUnitDetail ToDragonOddsUnitDetail() =>
            new() { Rarity = Rarity, UnitList = DragonList };
    }
}
