using DragaliaAPI.Features.Summoning;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DragaliaAPI.Test.Features.Summon;

public class SummonOddsServiceTest
{
    // Even though we are using decimal, some error remains because the reference rates sourced from the game were rounded.
    private const decimal AssertionPrecision = 0.00005m;

    private readonly IOptionsMonitor<SummonBannerOptions> optionsMonitor;
    private readonly SummonOddsService summonOddsService;

    public SummonOddsServiceTest()
    {
        this.optionsMonitor = Substitute.For<IOptionsMonitor<SummonBannerOptions>>();
        this.summonOddsService = new(this.optionsMonitor);
    }

    [Fact]
    public async Task GetUnitRates_FiveStarPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.FaeblessedTobias],
                        PickupDragons = [Dragons.Simurgh],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.FaeblessedTobias, 0.005m),
                    new UnitRate(Dragons.Simurgh, 0.008m)
                ]
            );

        normalRates.Should().NotContain(x => x.Id == (int)Charas.FaeblessedTobias);
        normalRates.Should().NotContain(x => x.Id == (int)Dragons.Simurgh);

        decimal expectedOffPickupRate = 0.04m - 0.005m - 0.008m;
        normalRates
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(expectedOffPickupRate, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0855m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0745m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.48m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.32m, AssertionPrecision);
    }

    [Fact]
    public async Task GetUnitRates_MultiFiveStarCharaPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.GalaNedrick, Charas.Akasha, Charas.Eirene],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.GalaNedrick, 0.005m),
                    new UnitRate(Charas.Akasha, 0.005m),
                    new UnitRate(Charas.Eirene, 0.005m)
                ]
            );

        decimal expectedOffPickupRate = 0.04m - 0.005m - 0.005m - 0.005m;
        normalRates
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(expectedOffPickupRate, AssertionPrecision);

        normalRates
            .Where(x => x.Rarity == 4)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.16m, AssertionPrecision);

        normalRates
            .Where(x => x.Rarity == 3)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.8m, AssertionPrecision);
    }

    [Fact]
    public async Task GetUnitRates_Gala_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.GalaZena, Charas.GalaRanzal],
                        PickupDragons = [Dragons.GalaBeastCiella]
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.GalaZena, 0.005m),
                    new UnitRate(Charas.GalaRanzal, 0.005m),
                    new UnitRate(Dragons.GalaBeastCiella, 0.008m)
                ]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.06m, AssertionPrecision);

        combined
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0855m, AssertionPrecision);
        combined
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0745m, AssertionPrecision);

        combined
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.47m, AssertionPrecision);
        combined
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.31m, AssertionPrecision);
    }

    [Fact]
    public async Task GetUnitRates_Gala_AddsLimitedUnits()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.GalaZena, Charas.GalaRanzal],
                        PickupDragons = [Dragons.GalaBeastCiella]
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.GalaZena, 0.005m),
                    new UnitRate(Charas.GalaRanzal, 0.005m),
                    new UnitRate(Dragons.GalaBeastCiella, 0.008m)
                ]
            );

        List<Charas> offPickupCharaIds = normalRates
            .Where(x => x is { EntityType: EntityTypes.Chara, Rarity: 5 })
            .Select(x => (Charas)x.Id)
            .ToList();

        offPickupCharaIds.Should().NotContain(Charas.GalaRanzal);
        offPickupCharaIds.Should().NotContain(Charas.GalaZena);

        offPickupCharaIds.Should().Contain(Charas.GalaSarisse);
        offPickupCharaIds.Should().Contain(Charas.GalaMym);
        offPickupCharaIds.Should().Contain(Charas.GalaCleo);
        offPickupCharaIds.Should().Contain(Charas.GalaPrince);
        offPickupCharaIds.Should().Contain(Charas.GalaElisanne);
        offPickupCharaIds.Should().Contain(Charas.GalaLuca);
        offPickupCharaIds.Should().Contain(Charas.GalaAlex);
        offPickupCharaIds.Should().Contain(Charas.GalaLeif);
        offPickupCharaIds.Should().Contain(Charas.GalaLaxi);
        offPickupCharaIds.Should().Contain(Charas.GalaLeonidas);
        offPickupCharaIds.Should().Contain(Charas.GalaChelle);
        offPickupCharaIds.Should().Contain(Charas.GalaNotte);
        offPickupCharaIds.Should().Contain(Charas.GalaMascula);
        offPickupCharaIds.Should().Contain(Charas.GalaAudric);
        offPickupCharaIds.Should().Contain(Charas.GalaZethia);
        offPickupCharaIds.Should().Contain(Charas.GalaGatov);
        offPickupCharaIds.Should().Contain(Charas.GalaEmile);
        offPickupCharaIds.Should().Contain(Charas.GalaNedrick);

        List<Dragons> offPickupDragonIds = normalRates
            .Where(x => x is { EntityType: EntityTypes.Dragon, Rarity: 5 })
            .Select(x => (Dragons)x.Id)
            .ToList();

        offPickupDragonIds.Should().NotContain(Dragons.GalaBeastCiella);

        offPickupDragonIds.Should().Contain(Dragons.GalaMars);
        offPickupDragonIds.Should().Contain(Dragons.GalaCatSith);
        offPickupDragonIds.Should().Contain(Dragons.GalaThor);
        offPickupDragonIds.Should().Contain(Dragons.GalaRebornPoseidon);
        offPickupDragonIds.Should().Contain(Dragons.GalaRebornZephyr);
        offPickupDragonIds.Should().Contain(Dragons.GalaRebornJeanne);
        offPickupDragonIds.Should().Contain(Dragons.GalaRebornNidhogg);
        offPickupDragonIds.Should().Contain(Dragons.GalaBeastVolk);
        offPickupDragonIds.Should().Contain(Dragons.GalaBahamut);
        offPickupDragonIds.Should().Contain(Dragons.GalaChronosNyx);
    }

    [Fact]
    public async Task GetUnitRates_DoesNotContainRestrictedCharas()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners = [new Banner() { Id = 1, PickupCharas = [Charas.BeauticianZardin], }]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> combinedRates = [.. rates.PickupRates, .. rates.NormalRates];

        Charas[] unexpectedCharas =
        [
            Charas.ThePrince,
            Charas.Elisanne,
            Charas.Ranzal,
            Charas.Luca,
            Charas.Cleo,
            Charas.Alex,
            Charas.Laxi,
            Charas.Zena,
            Charas.Chelle,
            Charas.GalaSarisse,
            Charas.GalaMym,
            Charas.GalaCleo,
            Charas.GalaPrince,
            Charas.GalaElisanne,
            Charas.GalaLuca,
            Charas.GalaAlex,
            Charas.GalaLeif,
            Charas.GalaLaxi,
            Charas.GalaLeonidas,
            Charas.GalaChelle,
            Charas.GalaNotte,
            Charas.GalaMascula,
            Charas.GalaAudric,
            Charas.GalaZethia,
            Charas.GalaGatov,
            Charas.GalaEmile,
            Charas.GalaNedrick,
            Charas.HalloweenEdward,
            Charas.HalloweenElisanne,
            Charas.HalloweenAlthemia,
            Charas.DragonyuleCleo,
            Charas.DragonyuleNefaria,
            Charas.DragonyuleXander,
            Charas.Addis,
            Charas.Ieyasu,
            Charas.Sazanka,
            Charas.HalloweenMym,
            Charas.HalloweenLowen,
            Charas.HalloweenOdetta,
            Charas.DragonyuleXainfried,
            Charas.DragonyuleMalora,
            Charas.Nobunaga,
            Charas.Mitsuhide,
            Charas.Chitose,
            Charas.ValentinesMelody,
            Charas.ValentinesAddis,
            Charas.HalloweenAkasha,
            Charas.HalloweenMelsa,
            Charas.DragonyuleLily,
            Charas.DragonyuleVictor,
            Charas.Seimei,
            Charas.Yoshitsune,
            Charas.ValentinesChelsea,
            Charas.SummerMitsuhide,
            Charas.SummerIeyasu,
            Charas.HalloweenLaxi,
            Charas.HalloweenSylas,
            Charas.DragonyuleNevin,
            Charas.DragonyuleIlia,
            Charas.Shingen,
            Charas.Yukimura,
            Charas.Celliera,
            Charas.Melsa,
            Charas.Elias,
            Charas.Botan,
            Charas.SuFang,
            Charas.Felicia,
            Charas.XuanZang,
            Charas.SummerEstelle,
            Charas.MegaMan,
            Charas.Hanabusa,
            Charas.Aldred,
            Charas.WuKong,
            Charas.SummerAmane,
            Charas.ForagerCleo,
            Charas.Kuzunoha,
            Charas.SophiePersona,
            Charas.HumanoidMidgardsormr,
            Charas.SummerPrince,
            Charas.Izumo,
            Charas.Alfonse,
            Charas.Audric,
            Charas.Sharena,
            Charas.Mordecai,
            Charas.Harle,
            Charas.Origa,
            Charas.Marth,
            Charas.Fjorm,
            Charas.Alfonse,
            Charas.Veronica,
            Charas.MegaMan,
            Charas.HunterBerserker,
            Charas.HunterVanessa,
            Charas.HunterSarisse,
            Charas.Chrom,
            Charas.Sharena,
            Charas.Peony,
            Charas.Tiki,
            Charas.Mona,
            Charas.SophiePersona,
            Charas.Joker,
            Charas.Panther
        ];

        foreach (Charas c in unexpectedCharas)
        {
            combinedRates.Should().NotContain(x => x.Id == (int)c);
        }
    }

    [Fact]
    public async Task GetUnitRates_DoesNotContainRestrictedDragons()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners = [new Banner() { Id = 1, PickupCharas = [Charas.BeauticianZardin], }]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> combinedRates = [.. rates.PickupRates, .. rates.NormalRates];

        Dragons[] unexpectedDragons =
        [
            Dragons.Midgardsormr,
            Dragons.Mercury,
            Dragons.Brunhilda,
            Dragons.Jupiter,
            Dragons.Zodiark,
            Dragons.GalaMars,
            Dragons.GalaCatSith,
            Dragons.GalaThor,
            Dragons.GalaRebornPoseidon,
            Dragons.GalaRebornZephyr,
            Dragons.GalaRebornJeanne,
            Dragons.GalaRebornAgni,
            Dragons.GalaRebornNidhogg,
            Dragons.GalaBeastVolk,
            Dragons.GalaBahamut,
            Dragons.GalaChronosNyx,
            Dragons.HalloweenSilke,
            Dragons.DragonyuleJeanne,
            Dragons.Marishiten,
            Dragons.HalloweenMaritimus,
            Dragons.Daikokuten,
            Dragons.GozuTenno,
            Dragons.SummerMarishiten,
            Dragons.FudoMyoo,
            Dragons.Pele,
            Dragons.Sylvia,
            Dragons.Maritimus,
            Dragons.Shishimai,
            Dragons.PengLai,
            Dragons.Phantom,
            Dragons.Yulong,
            Dragons.Erasmus,
            Dragons.Ebisu,
            Dragons.Rathalos,
            Dragons.Barbatos,
            Dragons.ParallelZodiark,
            Dragons.HighMidgardsormr,
            Dragons.HighMercury,
            Dragons.HighBrunhilda,
            Dragons.HighJupiter,
            Dragons.HighZodiark,
            Dragons.BronzeFafnir,
            Dragons.SilverFafnir,
            Dragons.GoldFafnir,
            Dragons.MiniMids,
            Dragons.MiniZodi,
            Dragons.MiniHildy,
            Dragons.MiniMercs,
            Dragons.MiniJupi
        ];

        foreach (Dragons d in unexpectedDragons)
        {
            combinedRates.Should().NotContain(x => x.Id == (int)d);
        }
    }

    [Fact]
    public async Task GetUnitRates_LimitedUnit_AddsToPool()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners = [new Banner() { Id = 1, LimitedCharas = [Charas.Joker], }]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> normalRates = rates.NormalRates.ToList();

        normalRates.Should().Contain(x => x.Id == (int)Charas.Joker);
        normalRates.Should().NotContain(x => x.Id == (int)Charas.Mona);
    }

    [Fact]
    public async Task GetGuaranteeUnitRates_FiveStarPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.Eirene],
                        PickupDragons = [Dragons.Agni],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetGuaranteeUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [new UnitRate(Charas.Eirene, 0.005m), new UnitRate(Dragons.Agni, 0.008m)]
            );

        decimal expectedOffPickupRate = 0.04m - 0.005m - 0.008m;
        normalRates
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(expectedOffPickupRate, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.513m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.447m, AssertionPrecision);

        normalRates.Should().NotContain(x => x.Rarity == 3);
    }

    [Fact]
    public async Task GetGuaranteeUnitRates_FiveStarPickup_Gala_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.GalaLeif],
                        PickupDragons = [Dragons.GalaRebornAgni],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetGuaranteeUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        // The sum is actually 0.9999000000000000000000000026M and needs lower precision to be accepted.
        // An error this small is acceptable - could be down to rounding in the reference figures.
        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision * 10);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.GalaLeif, 0.005m),
                    new UnitRate(Dragons.GalaRebornAgni, 0.008m)
                ]
            );

        decimal expectedOffPickupRate = 0.06m - 0.005m - 0.008m;
        normalRates
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(expectedOffPickupRate, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.5023m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.4376m, AssertionPrecision);

        normalRates.Should().NotContain(x => x.Rarity == 3);
    }

    [Fact]
    public async Task GetUnitRates_FourStarPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.KuHai],
                        PickupDragons = [Dragons.Roc],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [new UnitRate(Charas.KuHai, 0.035m), new UnitRate(Dragons.Roc, 0.035m)]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.04m, AssertionPrecision);

        combined
            .Where(x => x.Rarity == 4)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.16m, AssertionPrecision);

        combined
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.48m, AssertionPrecision);
        combined
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.32m, AssertionPrecision);
    }

    [Fact]
    public async Task GetUnitRates_FourStarPickup_Gala_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.KuHai, Charas.GalaAudric],
                        PickupDragons = [Dragons.Roc, Dragons.GalaBahamut],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.KuHai, 0.035m),
                    new UnitRate(Charas.GalaAudric, 0.005m),
                    new UnitRate(Dragons.Roc, 0.035m),
                    new UnitRate(Dragons.GalaBahamut, 0.008m)
                ]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.06m, AssertionPrecision);

        combined
            .Where(x => x.Rarity == 4)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.16m, AssertionPrecision);

        combined
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.47m, AssertionPrecision);
        combined
            .Where(x => x is { Rarity: 3, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.31m, AssertionPrecision);
    }

    [Fact]
    public async Task GetGuaranteeUnitRates_FourStarPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.KuHai],
                        PickupDragons = [Dragons.Roc],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetGuaranteeUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo([new UnitRate(Charas.KuHai, 0.21m), new UnitRate(Dragons.Roc, 0.21m)]);

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.04m, AssertionPrecision);

        combined
            .Where(x => x.Rarity == 4)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.96m, AssertionPrecision);

        normalRates.Should().NotContain(x => x.Rarity == 3);
    }

    [Fact]
    public async Task GetGuaranteeUnitRates_FourStarPickup_Gala_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.KuHai, Charas.GalaCleo],
                        PickupDragons = [Dragons.Roc, Dragons.GalaChronosNyx],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetGuaranteeUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.KuHai, 0.205625m),
                    new UnitRate(Charas.GalaCleo, 0.005m),
                    new UnitRate(Dragons.Roc, 0.205625m),
                    new UnitRate(Dragons.GalaChronosNyx, 0.008m)
                ]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.06m, AssertionPrecision);

        combined
            .Where(x => x.Rarity == 4)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.94m, AssertionPrecision);

        normalRates.Should().NotContain(x => x.Rarity == 3);
    }

    [Fact]
    public async Task GetUnitRates_ThreeStarPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.Joe],
                        PickupDragons = [Dragons.PallidImp],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [new UnitRate(Charas.Joe, 0.04m), new UnitRate(Dragons.PallidImp, 0.04m),]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.04m, AssertionPrecision);

        combined
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0855m, AssertionPrecision);
        combined
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0745m, AssertionPrecision);

        combined
            .Where(x => x.Rarity == 3)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.8m, AssertionPrecision);
    }

    [Fact]
    public async Task GetUnitRates_ThreeStarPickup_Gala_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.Joe, Charas.GalaZethia],
                        PickupDragons = [Dragons.PallidImp, Dragons.GalaRebornJeanne],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.Joe, 0.04m),
                    new UnitRate(Charas.GalaZethia, 0.005m),
                    new UnitRate(Dragons.PallidImp, 0.04m),
                    new UnitRate(Dragons.GalaRebornJeanne, 0.008m)
                ]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.06m, AssertionPrecision);

        combined
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0855m, AssertionPrecision);
        combined
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.0745m, AssertionPrecision);

        combined
            .Where(x => x.Rarity == 3)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.78m, AssertionPrecision);
    }

    [Fact]
    public async Task GetGuaranteeUnitRates_ThreeStarPickup_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        PickupCharas = [Charas.Joe],
                        PickupDragons = [Dragons.PallidImp],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetGuaranteeUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates.Should().BeEmpty();

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.04m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.5130m, AssertionPrecision);
        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.4470m, AssertionPrecision);

        combined.Should().NotContain(x => x.Rarity == 3);
    }

    [Fact]
    public async Task GetGuaranteeUnitRates_ThreeStarPickup_Gala_ProducesExpectedRates()
    {
        this.optionsMonitor.CurrentValue.Returns(
            new SummonBannerOptions()
            {
                Banners =
                [
                    new Banner()
                    {
                        Id = 1,
                        IsGala = true,
                        PickupCharas = [Charas.Joe, Charas.GalaZethia],
                        PickupDragons = [Dragons.PallidImp, Dragons.GalaRebornJeanne],
                    }
                ]
            }
        );

        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rates =
            await this.summonOddsService.GetGuaranteeUnitRates(1);

        List<UnitRate> pickupRates = rates.PickupRates.ToList();
        List<UnitRate> normalRates = rates.NormalRates.ToList();
        List<UnitRate> combined = [.. pickupRates, .. normalRates];

        // Similar precision issues here.
        combined.Sum(x => x.Rate).Should().BeApproximately(1m, AssertionPrecision * 10);
        combined.Should().OnlyHaveUniqueItems(x => new { x.Id, x.EntityType });

        pickupRates
            .Should()
            .BeEquivalentTo(
                [
                    new UnitRate(Charas.GalaZethia, 0.005m),
                    new UnitRate(Dragons.GalaRebornJeanne, 0.008m)
                ]
            );

        combined
            .Where(x => x.Rarity == 5)
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.06m, AssertionPrecision);

        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Chara })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.5023m, AssertionPrecision);
        normalRates
            .Where(x => x is { Rarity: 4, EntityType: EntityTypes.Dragon })
            .Sum(x => x.Rate)
            .Should()
            .BeApproximately(0.4376m, AssertionPrecision);

        combined.Should().NotContain(x => x.Rarity == 3);
    }
}
