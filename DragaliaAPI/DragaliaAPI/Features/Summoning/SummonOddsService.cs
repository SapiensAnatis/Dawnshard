using System.Diagnostics;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Summoning;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

public class SummonOddsService(IOptionsMonitor<SummonBannerOptions> optionsMonitor)
{
    private readonly SummonBannerOptions summonOptions = optionsMonitor.CurrentValue;

    // Public for actual summon roll later
    // ReSharper disable once MemberCanBePrivate.Global
    public (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) GetUnitRates(
        int bannerId
    )
    {
        Banner? banner = this.summonOptions.Banners.FirstOrDefault(x => x.Id == bannerId);

        if (banner is null)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Banner ID {bannerId} was not found"
            );
        }

        List<CharaData> charaPool = Enum.GetValues<Charas>()
            .Where(x => IsCharaInBannerRegularPool(x, banner))
            .Select(x => MasterAsset.CharaData[x])
            .ToList();
        List<DragonData> dragonPool = Enum.GetValues<Dragons>()
            .Where(x => IsDragonInBannerRegularPool(x, banner))
            .Select(x => MasterAsset.DragonData[x])
            .ToList();

        List<CharaData> pickupCharaPool = banner
            .PickupCharas.Select(x => MasterAsset.CharaData[x])
            .ToList();
        List<DragonData> pickupDragonPool = banner
            .PickupDragons.Select(x => MasterAsset.DragonData[x])
            .ToList();

        BaseRateData rateData = GetBaseRates(pickupCharaPool, pickupDragonPool, banner.IsGala);

        return (
            GetPickupUnitRarities(pickupCharaPool, pickupDragonPool),
            GetUnitRarities(rateData, charaPool, dragonPool)
        );
    }

    // Public for actual summon roll later
    // ReSharper disable once MemberCanBePrivate.Global
    public (
        IEnumerable<UnitRate> PickupRates,
        IEnumerable<UnitRate> NormalRates
    ) GetGuaranteeUnitRates(int bannerId)
    {
        Banner? banner = this.summonOptions.Banners.FirstOrDefault(x => x.Id == bannerId);

        if (banner is null)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Banner ID {bannerId} was not found"
            );
        }

        List<CharaData> charaPool = Enum.GetValues<Charas>()
            .Where(x => IsCharaInBannerRegularPool(x, banner))
            .Select(x => MasterAsset.CharaData[x])
            .Where(x => x.Rarity >= 4)
            .ToList();
        List<DragonData> dragonPool = Enum.GetValues<Dragons>()
            .Where(x => IsDragonInBannerRegularPool(x, banner))
            .Select(x => MasterAsset.DragonData[x])
            .Where(x => x.Rarity >= 4)
            .ToList();

        List<CharaData> pickupCharaPool = banner
            .PickupCharas.Select(x => MasterAsset.CharaData[x])
            .Where(x => x.Rarity >= 4)
            .ToList();
        List<DragonData> pickupDragonPool = banner
            .PickupDragons.Select(x => MasterAsset.DragonData[x])
            .Where(x => x.Rarity >= 4)
            .ToList();

        BaseRateData rateData = GetGuaranteeBaseRates(
            pickupCharaPool,
            pickupDragonPool,
            banner.IsGala
        );

        return (
            GetGuaranteePickupUnitRarities(pickupCharaPool, pickupDragonPool),
            GetUnitRarities(rateData, charaPool, dragonPool)
        );
    }

    public OddsRate GetNormalOddsRate(int bannerId)
    {
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            this.GetUnitRates(bannerId);

        // TODO: Factor in pity rate
        Dictionary<int, RarityList> pickupRarityLists =
            new()
            {
                [5] = new(5, true, [], []),
                [4] = new(4, true, [], []),
                [3] = new(3, true, [], []),
            };

        Dictionary<int, RarityList> rarityLists =
            new()
            {
                [5] = new(5, false, [], []),
                [4] = new(4, false, [], []),
                [3] = new(3, false, [], []),
            };

        foreach (UnitRate rate in rates.PickupRates)
            PopulateRarityDict(rate, pickupRarityLists);

        foreach (UnitRate rate in rates.NormalRates)
            PopulateRarityDict(rate, rarityLists);

        List<RarityList> combined =
        [
            .. pickupRarityLists.Values.Where(x => !x.IsEmpty),
            .. rarityLists.Values
        ];

        return new OddsRate()
        {
            RarityList = combined
                .GroupBy(x => x.Rarity)
                .Select(x => new AtgenRarityList()
                {
                    Rarity = x.Key,
                    TotalRate = x.Sum(y => y.CharaRate + y.DragonRate).ToPercentageString()
                }),
            RarityGroupList = combined.Select(x => x.ToRarityGroupList()),
            Unit = new()
            {
                CharaOddsList = combined.Select(x => x.ToAdvOddsUnitDetail()),
                DragonOddsList = combined.Select(x => x.ToDragonOddsUnitDetail()),
            }
        };
    }

    public OddsRate GetGuaranteeOddsRate(int bannerId)
    {
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            this.GetGuaranteeUnitRates(bannerId);

        Dictionary<int, RarityList> pickupRarityLists =
            new() { [5] = new(5, true, [], []), [4] = new(4, true, [], []), };

        Dictionary<int, RarityList> rarityLists =
            new() { [5] = new(5, false, [], []), [4] = new(4, false, [], []), };

        foreach (UnitRate rate in rates.PickupRates)
            PopulateRarityDict(rate, pickupRarityLists);

        foreach (UnitRate rate in rates.NormalRates)
            PopulateRarityDict(rate, rarityLists);

        List<RarityList> combined =
        [
            .. pickupRarityLists.Values.Where(x => !x.IsEmpty),
            .. rarityLists.Values
        ];

        return new OddsRate()
        {
            RarityList = combined
                .GroupBy(x => x.Rarity)
                .Select(x => new AtgenRarityList()
                {
                    Rarity = x.Key,
                    TotalRate = x.Sum(y => y.CharaRate + y.DragonRate).ToPercentageString()
                }),
            RarityGroupList = combined.Select(x => x.ToRarityGroupList()),
            Unit = new()
            {
                CharaOddsList = combined.Select(x => x.ToAdvOddsUnitDetail()),
                DragonOddsList = combined.Select(x => x.ToDragonOddsUnitDetail()),
            }
        };
    }

    private static void PopulateRarityDict(UnitRate rate, Dictionary<int, RarityList> dict)
    {
        RarityList list = dict[rate.Rarity];

        if (rate.EntityType == EntityTypes.Chara)
        {
            list.CharaList.Add(rate);
            list.CharaRate += rate.Rate;
        }
        else if (rate.EntityType == EntityTypes.Dragon)
        {
            list.DragonList.Add(rate);
            list.DragonRate += rate.Rate;
        }
        else
        {
            throw new UnreachableException(
                $"Invalid rarity entity type ({rate.EntityType}) or rarity ({rate.Rarity})"
            );
        }
    }

    private static IEnumerable<UnitRate> GetPickupUnitRarities(
        List<CharaData> pickupCharaPool,
        List<DragonData> pickupDragonPool
    )
    {
        int totalPickupFourStar =
            pickupCharaPool.Count(x => x.Rarity == 4) + pickupDragonPool.Count(x => x.Rarity == 4);

        foreach (CharaData data in pickupCharaPool)
        {
            decimal rate = data.Rarity switch
            {
                5 => 0.005m,
                4 => 0.07m / totalPickupFourStar,
                3 => 0.04m,
                _
                    => throw new UnreachableException(
                        $"Invalid rarity {data.Rarity} for character {data.Id}"
                    )
            };

            yield return new UnitRate(data.Id, rate);
        }

        foreach (DragonData data in pickupDragonPool)
        {
            decimal rate = data.Rarity switch
            {
                5 => 0.008m,
                4 => 0.07m / totalPickupFourStar,
                3 => 0.04m,
                _
                    => throw new UnreachableException(
                        $"Invalid rarity {data.Rarity} for character {data.Id}"
                    )
            };

            yield return new UnitRate(data.Id, rate);
        }
    }

    private static IEnumerable<UnitRate> GetGuaranteePickupUnitRarities(
        List<CharaData> pickupCharaPool,
        List<DragonData> pickupDragonPool
    )
    {
        int totalPickupFourStar =
            pickupCharaPool.Count(x => x.Rarity == 4) + pickupDragonPool.Count(x => x.Rarity == 4);

        foreach (CharaData data in pickupCharaPool)
        {
            decimal rate = data.Rarity switch
            {
                5 => 0.005m,
                4 => 0.42m / totalPickupFourStar,
                _
                    => throw new UnreachableException(
                        $"Invalid guarantee rarity {data.Rarity} for character {data.Id}"
                    )
            };

            yield return new UnitRate(data.Id, rate);
        }

        foreach (DragonData data in pickupDragonPool)
        {
            decimal rate = data.Rarity switch
            {
                5 => 0.008m,
                4 => 0.42m / totalPickupFourStar,
                _
                    => throw new UnreachableException(
                        $"Invalid guarantee rarity {data.Rarity} for character {data.Id}"
                    )
            };

            yield return new UnitRate(data.Id, rate);
        }
    }

    private static IEnumerable<UnitRate> GetUnitRarities(
        BaseRateData rateData,
        List<CharaData> charaPool,
        List<DragonData> dragonPool
    )
    {
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
        if (banner.PickupCharas.Contains(chara))
            return false; // They are in the pickup pool instead.

        UnitAvailability availability = chara.GetAvailability();

        return availability switch
        {
            UnitAvailability.Permanent => true,
            UnitAvailability.Gala => banner.IsGala,
            UnitAvailability.Limited => banner.LimitedCharas.Contains(chara),
            _ => false
        };
    }

    private static bool IsDragonInBannerRegularPool(Dragons dragon, Banner banner)
    {
        if (banner.PickupDragons.Contains(dragon))
            return false; // They are in the pickup pool instead.

        UnitAvailability availability = dragon.GetAvailability();

        return availability switch
        {
            UnitAvailability.Permanent => true,
            UnitAvailability.Gala => banner.IsGala,
            UnitAvailability.Limited => banner.LimitedDragons.Contains(dragon),
            _ => false
        };
    }

    private static BaseRateData GetBaseRates(
        List<CharaData> pickupCharaData,
        List<DragonData> pickupDragonData,
        bool isGala
    )
    {
        decimal fiveStarRate = isGala ? 0.06m : 0.04m;

        fiveStarRate -= pickupCharaData.Count(x => x.Rarity == 5) * 0.005m;
        fiveStarRate -= pickupDragonData.Count(x => x.Rarity == 5) * 0.008m;

        decimal fourStarAdvRate;
        decimal fourStarDragonRate;

        bool anyFourStarPickup =
            pickupCharaData.Any(x => x.Rarity == 4) || pickupDragonData.Any(x => x.Rarity == 4);

        if (anyFourStarPickup)
        {
            fourStarAdvRate = 0.045m;
            fourStarDragonRate = 0.045m;
        }
        else
        {
            fourStarAdvRate = 0.0855m;
            fourStarDragonRate = 0.0745m;
        }

        int threeStarPickupCount =
            pickupCharaData.Count(x => x.Rarity == 3) + pickupCharaData.Count(x => x.Rarity == 3);

        decimal threeStarAdvRate;
        decimal threeStarDragonRate;

        if (threeStarPickupCount > 0)
        {
            decimal threeStarRate = isGala ? 0.8m : 0.78m;
            threeStarRate -= pickupCharaData.Count(x => x.Rarity == 5);

            threeStarAdvRate = threeStarRate / 2m;
            threeStarDragonRate = threeStarRate / 2m;
        }
        else
        {
            threeStarAdvRate = isGala ? 0.47m : 0.48m;
            threeStarDragonRate = isGala ? 0.31m : 0.32m;
        }

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

    private static BaseRateData GetGuaranteeBaseRates(
        List<CharaData> pickupCharaData,
        List<DragonData> pickupDragonData,
        bool isGala
    )
    {
        decimal fiveStarRate = isGala ? 0.06m : 0.04m;

        fiveStarRate -= pickupCharaData.Count(x => x.Rarity == 5) * 0.005m;
        fiveStarRate -= pickupDragonData.Count(x => x.Rarity == 5) * 0.008m;

        decimal fourStarAdvRate;
        decimal fourStarDragonRate;

        bool anyFourStarPickup =
            pickupCharaData.Any(x => x.Rarity == 4) || pickupDragonData.Any(x => x.Rarity == 4);

        if (anyFourStarPickup)
        {
            fourStarAdvRate = 0.22m;
            fourStarDragonRate = 0.22m;
        }
        else
        {
            fourStarAdvRate = 0.513m;
            fourStarDragonRate = 0.447m;
        }

        return new()
        {
            FiveStarAdvRate = fiveStarRate / 2,
            FiveStarDragonRate = fiveStarRate / 2,
            FourStarAdvRate = fourStarAdvRate,
            FourStarDragonRate = fourStarDragonRate,
            ThreeStarAdvRate = 0m,
            ThreeStarDragonRate = 0m
        };
    }

    private static PoolSizeData GetPoolSizeByRarity(IEnumerable<IUnitData> pool)
    {
        int fiveStarPoolSize = 0;
        int fourStarPoolSize = 0;
        int threeStarPoolSize = 0;

        foreach (IUnitData data in pool)
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
        bool Pickup,
        List<UnitRate> DragonList,
        List<UnitRate> CharaList
    )
    {
        public bool IsEmpty => DragonList.Count == 0 && CharaList.Count == 0;

        public decimal CharaRate { get; set; }

        public decimal DragonRate { get; set; }

        public AtgenRarityGroupList ToRarityGroupList() =>
            new()
            {
                Rarity = this.Rarity,
                Pickup = this.Pickup,
                TotalRate = (this.CharaRate + this.DragonRate).ToPercentageString(),
                DragonRate = this.DragonRate.ToPercentageString(),
                CharaRate = this.CharaRate.ToPercentageString()
            };

        public OddsUnitDetail ToAdvOddsUnitDetail() =>
            new()
            {
                Pickup = this.Pickup,
                Rarity = this.Rarity,
                UnitList = this.CharaList.Select(x => x.ToAtgenUnitList())
            };

        public OddsUnitDetail ToDragonOddsUnitDetail() =>
            new()
            {
                Pickup = this.Pickup,
                Rarity = this.Rarity,
                UnitList = this.DragonList.Select(x => x.ToAtgenUnitList())
            };
    }
}
