using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Summoning;

public class SummonOddsService(IOptionsMonitor<SummonBannerOptions> optionsMonitor)
{
    private const double FourStarRate = 0.16;

    private const double ThreeStarStandardRate = 0.8;
    private const double ThreeStarGalaRate = 0.78;

    private readonly SummonBannerOptions summonOptions = optionsMonitor.CurrentValue;

    public Dictionary<int, RarityGroup>? GetRarityGroups(int bannerId)
    {
        Banner? banner = this.summonOptions.Banners.FirstOrDefault(x => x.Id == bannerId);

        if (banner is null)
            return null;
    }

    private static RarityGroup CalculateFiveStarRates(Banner banner)
    {
        const double fiveStarStandardRate = 0.04;
        const double fiveStarGalaRate = 0.06;
        const double focusAdvRate = 0.005;
        const double focusDragonRate = 0.008;

        double baseRate = banner.IsGala ? fiveStarGalaRate : fiveStarStandardRate;
        double offFocusRate = baseRate;

        offFocusRate -= banner.FeaturedAdventurers.SelectRarity().Count(x => x == 5) * focusAdvRate;
        offFocusRate -= banner.FeaturedDragons.SelectRarity().Count(x => x == 5) * focusDragonRate;

        if (offFocusRate < 0)
        {
            throw new InvalidOperationException(
                "Banner configuration has too many featured five star units"
            );
        }

        double advRate = offFocusRate / 2;
        double dragonRate = offFocusRate / 2;

        advRate += banner.FeaturedAdventurers.Count * focusAdvRate;
        dragonRate += banner.FeaturedAdventurers.Count * focusDragonRate;

        return new RarityGroup(5, baseRate, advRate, dragonRate);
    }

    private static List<UnitRarity> CalculateRarityList(Banner banner)
    {
        const int permanentAdvCount = 178;
        const int permanentDragonCount = 78;

        List<UnitRarity> result = new(permanentAdvCount + permanentDragonCount);
    }

    private static HashSet<UnitRarity> CalculateFiveStarRarities(Banner banner)
    {
        const double fiveStarStandardRate = 0.04;
        const double fiveStarGalaRate = 0.06;
        const double focusAdvRate = 0.005;
        const double focusDragonRate = 0.008;

        HashSet<UnitRarity> set = new(UnitRarityComparer.Instance);

        double baseRate = banner.IsGala ? fiveStarGalaRate : fiveStarStandardRate;
        double offFocusRate = baseRate;

        foreach (
            Charas adventurer in banner.FeaturedAdventurers.Where(x =>
                MasterAsset.CharaData[x].Rarity == 5
            )
        )
        {
            offFocusRate -= focusAdvRate;
            set.Add(new UnitRarity(adventurer, focusAdvRate));
        }

        foreach (
            Dragons dragons in banner.FeaturedDragons.Where(x =>
                MasterAsset.DragonData[x].Rarity == 5
            )
        )
        {
            offFocusRate -= focusDragonRate;
            set.Add(new UnitRarity(dragons, focusAdvRate));
        }
    }

    private class UnitRarityComparer : IEqualityComparer<UnitRarity>
    {
        public static UnitRarityComparer Instance { get; } = new();

        public bool Equals(UnitRarity? x, UnitRarity? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(UnitRarity obj) => obj.Id;
    }
}

file static class Extensions
{
    public static IEnumerable<int> SelectRarity(this IEnumerable<Charas> charasEnumerable) =>
        charasEnumerable.Select(x => MasterAsset.CharaData[x].Rarity);

    public static IEnumerable<int> SelectRarity(this IEnumerable<Dragons> dragonsEnumerable) =>
        dragonsEnumerable.Select(x => MasterAsset.DragonData[x].Rarity);
}
