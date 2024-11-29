using System.Net.Http.Json;
using DragaliaAPI.Features.Web;
using DragaliaAPI.Features.Web.TimeAttack.Models;

namespace DragaliaAPI.Integration.Test.Features.Web.TimeAttack;

public partial class TimeAttackTest : TestFixture
{
    public TimeAttackTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task GetQuests_ReturnsQuests()
    {
        await this.SeedTimeAttackData();

        List<TimeAttackQuest>? quests = await this.Client.GetFromJsonAsync<List<TimeAttackQuest>>(
            "/api/time_attack/quests",
            cancellationToken: TestContext.Current.CancellationToken
        );

        quests
            .Should()
            .BeEquivalentTo(
                [
                    new TimeAttackQuest() { Id = 227010104, IsCoop = false },
                    new TimeAttackQuest() { Id = 227010105, IsCoop = true },
                ],
                opts => opts.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task GetRankings_ReturnsRankings_FiltersDuplicateClearsFromSamePlayers()
    {
        await this.SeedTimeAttackData();

        OffsetPagedResponse<TimeAttackRanking>? rankings = await this.Client.GetFromJsonAsync<
            OffsetPagedResponse<TimeAttackRanking>
        >(
            "/api/time_attack/rankings/227010105",
            cancellationToken: TestContext.Current.CancellationToken
        );

        rankings?.Pagination.TotalCount.Should().Be(2);

        rankings
            ?.Data.Should()
            .BeEquivalentTo(
                [
                    new()
                    {
                        Rank = 1,
                        Time = 22.811f,
                        Players =
                        [
                            new() { Name = "Qwerby", Units = null! },
                            new() { Name = "Leom", Units = null! },
                            new() { Name = "Shiny", Units = null! },
                            new() { Name = "poopnut", Units = null! },
                        ],
                    },
                    // Second clear from above four is filtered
                    new TimeAttackRanking()
                    {
                        Rank = 2,
                        Time = 31.811f,
                        Players =
                        [
                            new() { Name = "Alicia", Units = null! },
                            new() { Name = "eze", Units = null! },
                            new() { Name = "OzpinXD", Units = null! },
                            new() { Name = "Euden", Units = null! },
                        ],
                    },
                ],
                opts => opts.For(x => x.Players).Exclude(x => x.Units)
            );
    }

    [Fact]
    public async Task GetRankings_ParsesTeamDataFromJson()
    {
        await this.SeedTimeAttackData();

        AssertionOptions.FormattingOptions.MaxLines = 10000;

        OffsetPagedResponse<TimeAttackRanking>? rankings = await this.Client.GetFromJsonAsync<
            OffsetPagedResponse<TimeAttackRanking>
        >(
            "/api/time_attack/rankings/227010104",
            cancellationToken: TestContext.Current.CancellationToken
        );

        rankings
            ?.Data.Should()
            .BeEquivalentTo(
                [
                    new TimeAttackRanking()
                    {
                        Rank = 1,
                        Time = 22.811f,
                        Players =
                        [
                            new()
                            {
                                Name = "Qwerby",
                                Units =
                                [
                                    new()
                                    {
                                        Chara = new()
                                        {
                                            BaseId = 100044,
                                            Id = 10350104,
                                            VariationId = 2,
                                        },
                                        Crests =
                                        [
                                            new()
                                            {
                                                BaseId = 400435,
                                                Id = AbilityCrestId.MeandMyBestie,
                                                ImageNum = 2,
                                            },
                                            new()
                                            {
                                                BaseId = 400402,
                                                Id = AbilityCrestId.BrothersinArms,
                                                ImageNum = 2,
                                            },
                                            new()
                                            {
                                                BaseId = 400417,
                                                Id = AbilityCrestId.FelyneHospitality,
                                                ImageNum = 2,
                                            },
                                            new()
                                            {
                                                BaseId = 400282,
                                                Id = AbilityCrestId.HalidomGrooms,
                                                ImageNum = 2,
                                            },
                                            new()
                                            {
                                                BaseId = 400498,
                                                Id = AbilityCrestId.AButlersSmile,
                                                ImageNum = 2,
                                            },
                                            new()
                                            {
                                                BaseId = 400301,
                                                Id =
                                                    AbilityCrestId.ThunderswiftLordsBladeLancesBoon,
                                                ImageNum = 1,
                                            },
                                            new()
                                            {
                                                BaseId = 400181,
                                                Id =
                                                    AbilityCrestId.AppleliciousDreamsButterflysBoon,
                                                ImageNum = 1,
                                            },
                                        ],
                                        Dragon = new()
                                        {
                                            BaseId = 210177,
                                            Id = 20050119,
                                            VariationId = 1,
                                        },
                                        Position = 1,
                                        SharedSkills =
                                        [
                                            new()
                                            {
                                                Id = 105302022,
                                                SkillLv4IconName = "Icon_Skill_031",
                                            },
                                            new()
                                            {
                                                Id = 102504021,
                                                SkillLv4IconName = "Icon_Skill_078",
                                            },
                                        ],
                                        Talisman = new()
                                        {
                                            Ability1Id = 340000020,
                                            Ability2Id = 340000132,
                                            Element = UnitElement.Fire,
                                            Id = Talismans.SummerMitsuhide,
                                            WeaponType = WeaponTypes.Dagger,
                                        },
                                        Weapon = new()
                                        {
                                            BaseId = 303147,
                                            ChangeSkillId1 = 303601041,
                                            FormId = 60101,
                                            Id = WeaponBodies.FlamerulersFang,
                                            VariationId = 1,
                                        },
                                    },
                                    new()
                                    {
                                        Chara = new()
                                        {
                                            BaseId = 110346,
                                            Id = 10250103,
                                            VariationId = 1,
                                        },
                                        Crests =
                                        [
                                            new()
                                            {
                                                BaseId = 400143,
                                                Id = AbilityCrestId.AHeartfeltGift,
                                                ImageNum = 1,
                                            },
                                            null,
                                            null,
                                            null,
                                            null,
                                            null,
                                            null,
                                        ],
                                        Dragon = null,
                                        Position = 2,
                                        SharedSkills =
                                        [
                                            new()
                                            {
                                                Id = 108405011,
                                                SkillLv4IconName = "Icon_Skill_029",
                                            },
                                            new()
                                            {
                                                Id = 104403011,
                                                SkillLv4IconName = "Icon_Skill_004",
                                            },
                                        ],
                                        Talisman = null,
                                        Weapon = new()
                                        {
                                            BaseId = 302148,
                                            ChangeSkillId1 = 302601041,
                                            FormId = 60101,
                                            Id = WeaponBodies.RagingConflagration,
                                            VariationId = 1,
                                        },
                                    },
                                    new()
                                    {
                                        Chara = new()
                                        {
                                            BaseId = 110004,
                                            Id = 10730101,
                                            VariationId = 1,
                                        },
                                        Crests = [null, null, null, null, null, null, null],
                                        Dragon = null,
                                        Position = 3,
                                        SharedSkills =
                                        [
                                            new()
                                            {
                                                Id = 108405011,
                                                SkillLv4IconName = "Icon_Skill_029",
                                            },
                                            new()
                                            {
                                                Id = 104403011,
                                                SkillLv4IconName = "Icon_Skill_004",
                                            },
                                        ],
                                        Talisman = null,
                                        Weapon = new()
                                        {
                                            BaseId = 307001,
                                            ChangeSkillId1 = 0,
                                            FormId = 19901,
                                            Id = WeaponBodies.BattlewornWand,
                                            VariationId = 1,
                                        },
                                    },
                                    new()
                                    {
                                        Chara = new()
                                        {
                                            BaseId = 100010,
                                            Id = 10450102,
                                            VariationId = 7,
                                        },
                                        Crests = [null, null, null, null, null, null, null],
                                        Dragon = null,
                                        Position = 4,
                                        SharedSkills =
                                        [
                                            new()
                                            {
                                                Id = 108405011,
                                                SkillLv4IconName = "Icon_Skill_029",
                                            },
                                            new()
                                            {
                                                Id = 104403011,
                                                SkillLv4IconName = "Icon_Skill_004",
                                            },
                                        ],
                                        Talisman = null,
                                        Weapon = new()
                                        {
                                            BaseId = 304001,
                                            ChangeSkillId1 = 0,
                                            FormId = 19901,
                                            Id = WeaponBodies.BattlewornAxe,
                                            VariationId = 1,
                                        },
                                    },
                                ],
                            },
                        ],
                    },
                ]
            );
    }
}
