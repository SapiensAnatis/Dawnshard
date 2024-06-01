using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

using UnitRateCollection = (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates);

public class SummonOddsService(
    IOptionsMonitor<SummonBannerOptions> optionsMonitor,
    ApiContext apiContext
)
{
    public OddsRate GetNormalOddsRate(int bannerId, int summonCountSinceLastFiveStar)
    {
        UnitRateCollection rates = this.GetUnitRates(bannerId, summonCountSinceLastFiveStar);

        return BuildOddsRate(rates, [5, 4, 3]);
    }

    public OddsRate? GetGuaranteeOddsRate(int bannerId, int summonCountSinceLastFiveStar)
    {
        if (
            optionsMonitor.CurrentValue.BannerDict.TryGetValue(bannerId, out Banner? banner)
            && banner.SummonType != SummonTypes.Normal
        )
        {
            // Certain special banners, like the 5* summon voucher ones, have no concept
            // of a guarantee rate -- because you can't execute a tenfold on them.
            // We must return null for these, otherwise the guarantee tab appears in
            // Japanese text.
            return null;
        }

        UnitRateCollection rates = this.GetGuaranteeUnitRates(
            bannerId,
            summonCountSinceLastFiveStar
        );

        return BuildOddsRate(rates, [5, 4]);
    }

    public UnitRateCollection GetUnitRates(int bannerId, int summonCountSinceLastFiveStar)
    {
        if (!optionsMonitor.CurrentValue.BannerDict.TryGetValue(bannerId, out Banner? banner))
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Banner ID {bannerId} was not found"
            );
        }

        return SummonOddsLogic.GetRates(banner, summonCountSinceLastFiveStar);
    }

    public UnitRateCollection GetGuaranteeUnitRates(int bannerId, int summonCountSinceLastFiveStar)
    {
        if (!optionsMonitor.CurrentValue.BannerDict.TryGetValue(bannerId, out Banner? banner))
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Banner ID {bannerId} was not found"
            );
        }

        return SummonOddsLogic.GetGuaranteeRates(banner, summonCountSinceLastFiveStar);
    }

    private static OddsRate BuildOddsRate(UnitRateCollection rates, int[] rarities)
    {
        Dictionary<int, RarityList> pickupRarityLists = rarities.ToDictionary(
            x => x,
            x => new RarityList() { Rarity = x, Pickup = true }
        );

        Dictionary<int, RarityList> rarityLists = rarities.ToDictionary(
            x => x,
            x => new RarityList() { Rarity = x }
        );

        foreach (UnitRate rate in rates.PickupRates)
        {
            PopulateRarityDict(rate, pickupRarityLists);
        }

        foreach (UnitRate rate in rates.NormalRates)
        {
            PopulateRarityDict(rate, rarityLists);
        }

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
                    TotalRate = x.Sum(y => y.CharaRate + y.DragonRate).ToPercentageString2Dp()
                }),
            RarityGroupList = combined.Select(x => x.ToRarityGroupList()),
            Unit = new()
            {
                CharaOddsList = combined.Select(x => x.ToAdvOddsUnitDetail()),
                DragonOddsList = combined.Select(x => x.ToDragonOddsUnitDetail()),
            }
        };

        static void PopulateRarityDict(UnitRate rate, Dictionary<int, RarityList> dict)
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
                throw new UnreachableException($"Invalid rarity entity type {rate.EntityType}");
            }
        }
    }

    public Task<int> GetSummonCountSinceLastFiveStar(int bannerId) =>
        // This will return 0 if no banner data is found, which is fine in this case.
        apiContext
            .PlayerBannerData.Where(x => x.SummonBannerId == bannerId)
            .Select(x => x.SummonCountSinceLastFiveStar)
            .FirstOrDefaultAsync();

    private class RarityList
    {
        public required int Rarity { get; init; }

        public bool Pickup { get; init; }

        public bool IsEmpty => DragonList.Count == 0 && CharaList.Count == 0;

        public List<UnitRate> DragonList { get; } = [];

        public List<UnitRate> CharaList { get; } = [];

        public decimal CharaRate { get; set; }

        public decimal DragonRate { get; set; }

        public AtgenRarityGroupList ToRarityGroupList() =>
            new()
            {
                Rarity = this.Rarity,
                Pickup = this.Pickup,
                TotalRate = (this.CharaRate + this.DragonRate).ToPercentageString2Dp(),
                DragonRate = this.DragonRate.ToPercentageString2Dp(),
                CharaRate = this.CharaRate.ToPercentageString2Dp()
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
