using System.Diagnostics;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Summoning;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Summoning;

/* Algorithm sourced from https://dragalialost.wiki/w/Summoning#Rarity_Distribution */

using UnitRateCollection = (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates);

public static class SummonOddsLogic
{
    public static UnitRateCollection GetRates(Banner banner, int numSummonsSinceLastFiveStar)
    {
        if (banner.OverrideCharaPool is not null)
        {
            return new UnitRateCollection
            {
                NormalRates = banner.OverrideCharaPool.Select(x => new UnitRate(
                    x,
                    1m / banner.OverrideCharaPool.Count
                )),
                PickupRates = [],
            };
        }

        if (banner.OverrideDragonPool is not null)
        {
            return new UnitRateCollection
            {
                NormalRates = banner.OverrideDragonPool.Select(x => new UnitRate(
                    x,
                    1m / banner.OverrideDragonPool.Count
                )),
                PickupRates = [],
            };
        }

        BaseRateData normalRates = GetBaseRates(banner);
        BaseRateData pickupRates = GetBasePickupRates(banner);

        (BaseRateData UpdatedNormalRates, BaseRateData UpdatedPickupRates) newRates = ApplyPityRate(
            rateData: normalRates,
            pickupRateData: pickupRates,
            banner: banner,
            numSummonsSinceLastFiveStar: numSummonsSinceLastFiveStar
        );

        List<CharaData> charaPool = MasterAsset
            .CharaData.Enumerable.Where(x => IsCharaInBannerRegularPool(x, banner))
            .ToList();
        List<DragonData> dragonPool = MasterAsset
            .DragonData.Enumerable.Where(x => IsDragonInBannerRegularPool(x, banner))
            .ToList();

        return (
            PickupRates: DistributeRates(
                banner.PickupCharaData,
                banner.PickupDragonData,
                newRates.UpdatedPickupRates
            ),
            NormalRates: DistributeRates(charaPool, dragonPool, newRates.UpdatedNormalRates)
        );
    }

    public static UnitRateCollection GetGuaranteeRates(
        Banner banner,
        int numSummonsSinceLastFiveStar
    )
    {
        if (banner.OverrideCharaPool is not null || banner.OverrideDragonPool is not null)
        {
            throw new ArgumentException(
                "Cannot get guarantee rates of a banner with an overridden pool",
                nameof(banner)
            );
        }

        BaseRateData normalRates = GetBaseGuaranteeRates(banner);
        BaseRateData pickupRates = GetBasePickupGuaranteeRates(banner);

        (BaseRateData UpdatedNormalRates, BaseRateData UpdatedPickupRates) newRates = ApplyPityRate(
            normalRates,
            pickupRates,
            banner,
            numSummonsSinceLastFiveStar
        );

        List<CharaData> charaPool = MasterAsset
            .CharaData.Enumerable.Where(x => IsCharaInBannerRegularPool(x, banner))
            .Where(x => x.Rarity >= 4)
            .ToList();
        List<DragonData> dragonPool = MasterAsset
            .DragonData.Enumerable.Where(x => IsDragonInBannerRegularPool(x, banner))
            .Where(x => x.Rarity >= 4)
            .ToList();

        List<CharaData> pickupCharaPool = banner.PickupCharaData.Where(x => x.Rarity >= 4).ToList();
        List<DragonData> pickupDragonPool = banner
            .PickupDragonData.Where(x => x.Rarity >= 4)
            .ToList();

        return (
            PickupRates: DistributeRates(
                pickupCharaPool,
                pickupDragonPool,
                newRates.UpdatedPickupRates
            ),
            NormalRates: DistributeRates(charaPool, dragonPool, newRates.UpdatedNormalRates)
        );
    }

    public static int GetSummonCountToPityIncrease(int numSummonsSinceLastFiveStar) =>
        10 - (numSummonsSinceLastFiveStar % 10);

    private static BaseRateData GetBaseRates(Banner banner)
    {
        decimal fiveStarRate = banner.IsGala ? 0.06m : 0.04m;

        fiveStarRate -= banner.PickupCharaData.Count(x => x.Rarity == 5) * 0.005m;
        fiveStarRate -= banner.PickupDragonData.Count(x => x.Rarity == 5) * 0.008m;

        decimal fourStarAdvRate;
        decimal fourStarDragonRate;

        bool anyFourStarPickup =
            banner.PickupCharaData.Any(x => x.Rarity == 4)
            || banner.PickupDragonData.Any(x => x.Rarity == 4);

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
            banner.PickupCharaData.Count(x => x.Rarity == 3)
            + banner.PickupDragonData.Count(x => x.Rarity == 3);

        decimal threeStarAdvRate;
        decimal threeStarDragonRate;

        if (threeStarPickupCount > 0)
        {
            decimal threeStarRate = banner.IsGala ? 0.78m : 0.8m;
            threeStarRate -= threeStarPickupCount * 0.04m;

            threeStarAdvRate = threeStarRate / 2m;
            threeStarDragonRate = threeStarRate / 2m;
        }
        else
        {
            threeStarAdvRate = banner.IsGala ? 0.47m : 0.48m;
            threeStarDragonRate = banner.IsGala ? 0.31m : 0.32m;
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

    private static BaseRateData GetBasePickupRates(Banner banner)
    {
        List<CharaData> pickupCharaData = banner
            .PickupCharas.Select(x => MasterAsset.CharaData[x])
            .ToList();
        List<DragonData> pickupDragonData = banner
            .PickupDragons.Select(x => MasterAsset.DragonData[x])
            .ToList();

        decimal fiveStarAdvRate = 0,
            fiveStarDragonRate = 0,
            fourStarAdvRate = 0,
            fourStarDragonRate = 0,
            threeStarAdvRate = 0,
            threeStarDragonRate = 0;

        PoolSizeData advPoolMetadata = GetPoolSizeByRarity(pickupCharaData);
        PoolSizeData dragonPoolMetadata = GetPoolSizeByRarity(pickupDragonData);

        if (advPoolMetadata.FiveStarPoolSize > 0)
        {
            fiveStarAdvRate = 0.005m * advPoolMetadata.FiveStarPoolSize;
        }
        if (dragonPoolMetadata.FiveStarPoolSize > 0)
        {
            fiveStarDragonRate = 0.008m * dragonPoolMetadata.FiveStarPoolSize;
        }

        switch (advPoolMetadata.FourStarPoolSize, dragonPoolMetadata.FourStarPoolSize)
        {
            case (> 0, > 0):
            {
                fourStarAdvRate = 0.035m;
                fourStarDragonRate = 0.035m;
                break;
            }
            case (> 0, 0):
            {
                fourStarAdvRate = 0.07m;
                break;
            }
            case (0, > 0):
            {
                fourStarDragonRate = 0.07m;
                break;
            }
            case (0, 0):
            {
                break;
            }
        }

        if (advPoolMetadata.ThreeStarPoolSize > 0)
        {
            threeStarAdvRate = 0.04m * advPoolMetadata.ThreeStarPoolSize;
        }
        if (dragonPoolMetadata.ThreeStarPoolSize > 0)
        {
            threeStarDragonRate = 0.04m * dragonPoolMetadata.ThreeStarPoolSize;
        }

        return new BaseRateData(
            FiveStarAdvRate: fiveStarAdvRate,
            FiveStarDragonRate: fiveStarDragonRate,
            FourStarAdvRate: fourStarAdvRate,
            FourStarDragonRate: fourStarDragonRate,
            ThreeStarAdvRate: threeStarAdvRate,
            ThreeStarDragonRate: threeStarDragonRate
        );
    }

    private static BaseRateData GetBaseGuaranteeRates(Banner banner)
    {
        decimal fiveStarRate = banner.IsGala ? 0.06m : 0.04m;

        fiveStarRate -= banner.PickupCharaData.Count(x => x.Rarity == 5) * 0.005m;
        fiveStarRate -= banner.PickupDragonData.Count(x => x.Rarity == 5) * 0.008m;

        decimal fourStarAdvRate;
        decimal fourStarDragonRate;

        bool anyFourStarPickup =
            banner.PickupCharaData.Any(x => x.Rarity == 4)
            || banner.PickupDragonData.Any(x => x.Rarity == 4);

        if (anyFourStarPickup)
        {
            fourStarAdvRate = banner.IsGala ? 0.264375m : 0.27m;
            fourStarDragonRate = banner.IsGala ? 0.264375m : 0.27m;
        }
        else
        {
            fourStarAdvRate = banner.IsGala ? 0.5023m : 0.5130m;
            fourStarDragonRate = banner.IsGala ? 0.4376m : 0.4470m;
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

    private static BaseRateData GetBasePickupGuaranteeRates(Banner banner)
    {
        BaseRateData rateData = GetBasePickupRates(banner);

        int fourStarAdvCount = banner.PickupCharaData.Count(x => x.Rarity == 4);
        int fourStarDragonCount = banner.PickupDragonData.Count(x => x.Rarity == 4);

        decimal fourStarAdvRate = 0,
            fourStarDragonRate = 0;

        decimal possibleFourStarRate = banner.IsGala ? 0.41125m : 0.42m;

        switch (fourStarAdvCount, fourStarDragonCount)
        {
            case (> 0, > 0):
            {
                fourStarAdvRate = possibleFourStarRate / 2;
                fourStarDragonRate = possibleFourStarRate / 2;
                break;
            }
            case (> 0, 0):
            {
                fourStarAdvRate = possibleFourStarRate;
                break;
            }
            case (0, > 0):
            {
                fourStarDragonRate = possibleFourStarRate;
                break;
            }

            case (0, 0):
            {
                break;
            }
        }

        return rateData with
        {
            FourStarAdvRate = fourStarAdvRate,
            FourStarDragonRate = fourStarDragonRate,
            ThreeStarAdvRate = 0,
            ThreeStarDragonRate = 0,
        };
    }

    private static IEnumerable<UnitRate> DistributeRates(
        IReadOnlyList<CharaData> charaPool,
        IReadOnlyList<DragonData> dragonPool,
        BaseRateData rateData
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

    private static bool IsCharaInBannerRegularPool(CharaData charaData, Banner banner)
    {
        if (!charaData.IsPlayable)
        {
            return false;
        }

        if (banner.PickupCharas.Contains(charaData.Id))
        {
            // They are in the pickup pool instead.
            return false;
        }

        UnitAvailability availability = charaData.GetAvailability();

        return availability switch
        {
            UnitAvailability.Permanent => true,
            UnitAvailability.Gala => banner.IsGala,
            UnitAvailability.Limited => banner.LimitedCharas.Contains(charaData.Id),
            _ => false
        };
    }

    private static bool IsDragonInBannerRegularPool(DragonData dragonData, Banner banner)
    {
        if (!dragonData.IsPlayable)
        {
            return false;
        }

        if (banner.PickupDragons.Contains(dragonData.Id))
        {
            return false;
        }

        UnitAvailability availability = dragonData.GetAvailability();

        return availability switch
        {
            UnitAvailability.Permanent => true,
            UnitAvailability.Gala => banner.IsGala,
            UnitAvailability.Limited => banner.LimitedDragons.Contains(dragonData.Id),
            _ => false
        };
    }

    private static (BaseRateData UpdatedNormalRates, BaseRateData UpdatedPickupRates) ApplyPityRate(
        BaseRateData rateData,
        BaseRateData pickupRateData,
        Banner banner,
        int numSummonsSinceLastFiveStar
    )
    {
        int pityAccumulations = numSummonsSinceLastFiveStar / 10;

        if (pityAccumulations == 0)
        {
            return (rateData, pickupRateData);
        }

        decimal fiveStarIncrease = pityAccumulations * 0.005m;
        decimal originalFiveStarRate = banner.IsGala ? 0.06m : 0.04m;

        BaseRateData newRateData;
        BaseRateData newPickupRateData;

        switch
            (
                banner.PickupCharaData.Any(x => x.Rarity == 5),
                banner.PickupDragonData.Any(x => x.Rarity == 5)
            )

        {
            case (true, true):
            {
                newPickupRateData = pickupRateData with
                {
                    FiveStarAdvRate = pickupRateData.FiveStarAdvRate + (fiveStarIncrease / 2),
                    FiveStarDragonRate = pickupRateData.FiveStarDragonRate + (fiveStarIncrease / 2),
                };
                newRateData = rateData;
                break;
            }
            case (true, false):
            {
                decimal pickupCharaProportion = Math.Round(
                    pickupRateData.FiveStarAdvRate / originalFiveStarRate,
                    2,
                    MidpointRounding.ToNegativeInfinity
                );
                decimal regularProportion = 1 - pickupCharaProportion;

                newPickupRateData = pickupRateData with
                {
                    FiveStarAdvRate =
                        pickupRateData.FiveStarAdvRate + (pickupCharaProportion * fiveStarIncrease)
                };

                // Can't work out how the remaining increase is split between characters and dragons.
                // This 1/5ths and 4/5ths split is purely empirical.
                newRateData = rateData with
                {
                    FiveStarAdvRate =
                        rateData.FiveStarAdvRate + (regularProportion * fiveStarIncrease * 0.2m),
                    FiveStarDragonRate =
                        rateData.FiveStarDragonRate + (regularProportion * fiveStarIncrease * 0.8m)
                };
                break;
            }
            case (false, true):
            {
                decimal pickupDragonProportion = Math.Round(
                    pickupRateData.FiveStarDragonRate / originalFiveStarRate,
                    2,
                    MidpointRounding.ToNegativeInfinity
                );
                decimal regularProportion = 1 - pickupDragonProportion;

                newPickupRateData = pickupRateData with
                {
                    FiveStarDragonRate =
                        pickupRateData.FiveStarDragonRate
                        + (pickupDragonProportion * fiveStarIncrease)
                };

                // Same as above empirical distribution, but inverted
                newRateData = rateData with
                {
                    FiveStarAdvRate =
                        rateData.FiveStarAdvRate + (regularProportion * fiveStarIncrease * 0.8m),
                    FiveStarDragonRate =
                        rateData.FiveStarDragonRate + (regularProportion * fiveStarIncrease * 0.2m)
                };
                break;
            }
            default:
            {
                newPickupRateData = pickupRateData;
                newRateData = rateData with
                {
                    FiveStarAdvRate = rateData.FiveStarAdvRate + (fiveStarIncrease / 2),
                    FiveStarDragonRate = rateData.FiveStarDragonRate + (fiveStarIncrease / 2)
                };
                break;
            }
        }

        if (newRateData is { ThreeStarAdvRate: 0, ThreeStarDragonRate: 0 })
        {
            // This is a guarantee rate, we should make room for the increase by subtracting from 4-star rates.

            // Subtract from the non-pickup rates here - not sure if the original game would distribute the
            // decrease among the pickup rates within lower rarities, as I don't have the data, but it seems
            // logical to take away from the 'least desirable' outcomes which would be non-pickup.
            newRateData = newRateData with
            {
                FourStarAdvRate = newRateData.FourStarAdvRate - (fiveStarIncrease / 2),
                FourStarDragonRate = newRateData.FourStarDragonRate - (fiveStarIncrease / 2),
            };
        }
        else if (newRateData is { ThreeStarAdvRate: > 0, ThreeStarDragonRate: > 0 })
        {
            newRateData = newRateData with
            {
                ThreeStarAdvRate = newRateData.ThreeStarAdvRate - (fiveStarIncrease / 2),
                ThreeStarDragonRate = newRateData.ThreeStarDragonRate - (fiveStarIncrease / 2),
            };
        }
        else
        {
            throw new UnreachableException("Could not determine how to subtract pity rate");
        }

        return (newRateData, newPickupRateData);
    }

    private readonly record struct PoolSizeData(
        int FiveStarPoolSize,
        int FourStarPoolSize,
        int ThreeStarPoolSize
    );

    private readonly record struct BaseRateData(
        decimal FiveStarAdvRate,
        decimal FiveStarDragonRate,
        decimal FourStarAdvRate,
        decimal FourStarDragonRate,
        decimal ThreeStarAdvRate,
        decimal ThreeStarDragonRate
    );
}
