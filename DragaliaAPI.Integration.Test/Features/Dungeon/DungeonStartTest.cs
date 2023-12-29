using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Enemy;
using Microsoft.EntityFrameworkCore;
using Snapshooter;
using Snapshooter.Xunit;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

/// <summary>
/// Tests <see cref="DragaliaAPI.Features.Dungeon.Start.DungeonStartController"/>.
/// </summary>
public class DungeonStartTest : TestFixture
{
    public DungeonStartTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override async Task Setup() => await this.ImportSave();

    [Fact]
    public async Task Start_OneTeam_HasExpectedPartyUnitList()
    {
        DungeonStartStartData response = (
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 1 },
                    quest_id = 100010103
                }
            )
        ).data;

        Snapshot.Match(response.ingame_data.party_info.party_unit_list, SnapshotOptions);

        response.ingame_data.party_info.party_unit_list.Should().HaveCount(4);
        response
            .ingame_data.party_info.party_unit_list.Should()
            .BeInAscendingOrder(x => x.position);
        response
            .ingame_data.party_info.party_unit_list.Should()
            .OnlyHaveUniqueItems(x => x.position);
    }

    [Fact]
    public async Task Start_TwoTeams_HasExpectedPartyUnitList()
    {
        DungeonStartStartData response = (
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 37, 38 },
                    quest_id = 100010103
                }
            )
        ).data;

        // Abuse of snapshots here is lazy, but the resulting JSON is thousands of lines long...
        Snapshot.Match(response.ingame_data.party_info.party_unit_list, SnapshotOptions);

        response.ingame_data.party_info.party_unit_list.Should().HaveCount(8);
        response
            .ingame_data.party_info.party_unit_list.Should()
            .BeInAscendingOrder(x => x.position);
        response
            .ingame_data.party_info.party_unit_list.Should()
            .OnlyHaveUniqueItems(x => x.position);
    }

    [Fact]
    public async Task Start_WeaponPassivesUnlocked_IncludedInPartyUnitList()
    {
        DungeonStartStartData response = (
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 38 },
                    quest_id = 100010103
                }
            )
        ).data;

        response
            .ingame_data.party_info.party_unit_list.First(
                x => x.chara_data!.chara_id == Charas.GalaMascula
            )
            .game_weapon_passive_ability_list.Should()
            .Contain(x => x.weapon_passive_ability_id == 1020211);
    }

    [Fact]
    public async Task StartAssignUnit_HasExpectedPartyList()
    {
        DungeonSkipStartAssignUnitRequest request =
            new()
            {
                quest_id = 100010103,
                request_party_setting_list = new List<PartySettingList>()
                {
                    new()
                    {
                        unit_no = 1,
                        chara_id = Charas.GalaLeonidas,
                        equip_weapon_body_id = WeaponBodies.Draupnir,
                        equip_dragon_key_id = (ulong)GetDragonKeyId(Dragons.Horus),
                        equip_crest_slot_type_1_crest_id_1 = AbilityCrests.PrimalCrisis,
                        equip_crest_slot_type_1_crest_id_2 = AbilityCrests.TheCutieCompetition,
                        equip_crest_slot_type_1_crest_id_3 = AbilityCrests.AnIndelibleDate,
                        equip_crest_slot_type_2_crest_id_1 = AbilityCrests.BeautifulGunman,
                        equip_crest_slot_type_2_crest_id_2 = AbilityCrests.DragonArcanum,
                        equip_talisman_key_id = (ulong)GetTalismanKeyId(Talismans.GalaLeonidas),
                        equip_crest_slot_type_3_crest_id_1 = AbilityCrests.AKnightsDreamAxesBoon,
                        equip_crest_slot_type_3_crest_id_2 = AbilityCrests.CrownofLightSerpentsBoon,
                        edit_skill_1_chara_id = Charas.GalaZethia,
                        edit_skill_2_chara_id = Charas.GalaMascula,
                    },
                    new()
                    {
                        unit_no = 2,
                        chara_id = Charas.GalaGatov,
                        equip_weapon_body_id = WeaponBodies.Mjoelnir,
                        equip_dragon_key_id = (ulong)GetDragonKeyId(Dragons.GalaMars),
                        equip_crest_slot_type_1_crest_id_1 = AbilityCrests.TheCutieCompetition,
                        equip_crest_slot_type_1_crest_id_2 = AbilityCrests.KungFuMasters,
                        equip_crest_slot_type_1_crest_id_3 = AbilityCrests.BondsBetweenWorlds,
                        equip_crest_slot_type_2_crest_id_1 = AbilityCrests.DragonArcanum,
                        equip_crest_slot_type_2_crest_id_2 = AbilityCrests.BeautifulNothingness,
                        equip_talisman_key_id = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                        equip_crest_slot_type_3_crest_id_1 =
                            AbilityCrests.TutelarysDestinyWolfsBoon,
                        equip_crest_slot_type_3_crest_id_2 =
                            AbilityCrests.TestamentofEternityFishsBoon,
                    }
                }
            };

        DungeonStartStartAssignUnitData response = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitData>(
                "/dungeon_start/start_assign_unit",
                request
            )
        ).data;

        // Only test the first two since the others are empty
        Snapshot.Match(response.ingame_data.party_info.party_unit_list.Take(2), SnapshotOptions);

        response.ingame_data.party_info.party_unit_list.Should().HaveCount(4);
        response
            .ingame_data.party_info.party_unit_list.Should()
            .Contain(x => x.chara_data!.chara_id == Charas.GalaLeonidas)
            .And.Contain(x => x.chara_data!.chara_id == Charas.GalaGatov);
    }

    [Theory]
    [InlineData("start")]
    [InlineData("start_assign_unit")]
    public async Task Start_InsufficientStamina_ReturnsError(string endpoint)
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaSingle, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaMulti, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaSingleUpdateTime, e => DateTimeOffset.UtcNow)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaMultiUpdateTime, e => DateTimeOffset.UtcNow)
        );

        (
            await Client.PostMsgpack<DungeonStartStartData>(
                $"/dungeon_start/{endpoint}",
                new DungeonStartStartRequest() { quest_id = 100010104, party_no_list = [1] },
                ensureSuccessHeader: false
            )
        )
            .data_headers.result_code.Should()
            .Be(ResultCode.QuestStaminaSingleShort);
    }

    [Fact]
    public async Task Start_ZeroStamina_FirstClearOfMainStory_Allows()
    {
        await this.ApiContext.PlayerQuests.ExecuteDeleteAsync();

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaSingle, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.StaminaMulti, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaSingleUpdateTime, e => DateTimeOffset.UtcNow)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            p => p.SetProperty(e => e.LastStaminaMultiUpdateTime, e => DateTimeOffset.UtcNow)
        );

        (
            await Client.PostMsgpack<DungeonStartStartData>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    quest_id = 100260101,
                    party_no_list = new List<int>() { 1 },
                },
                ensureSuccessHeader: false
            )
        ).data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task Start_ChronosClash_HasRareEnemy()
    {
        DragaliaResponse<DungeonStartStartData> response =
            await this.Client.PostMsgpack<DungeonStartStartData>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest() { quest_id = 204270302, party_no_list = [1] }
            );

        response.data.odds_info.enemy.Should().Contain(x => x.param_id == 204130320 && x.is_rare);
    }

    [Fact]
    public async Task Start_EarnEvent_EnemiesDuplicated()
    {
        const int earnEventQuestId = 229031201; // Repelling the Frosty Fiends: Standard (Solo)

        DragaliaResponse<DungeonStartStartData> response =
            await this.Client.PostMsgpack<DungeonStartStartData>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest() { quest_id = earnEventQuestId, party_no_list = [1] }
            );

        response.data.odds_info.enemy.Should().HaveCount(31);

        QuestData questData = MasterAsset.QuestData[earnEventQuestId];
        IEnumerable<int> enemies = MasterAsset
            .QuestEnemies[$"{questData.Scene01}/{questData.AreaName01}".ToLowerInvariant()]
            .Enemies[questData.VariationType];

        response.data.odds_info.enemy.Should().HaveCountGreaterThan(enemies.Count());
    }

    private static readonly Func<MatchOptions, MatchOptions> SnapshotOptions = opts =>
        opts.IgnoreField<long>("$..dragon_data.dragon_key_id")
            .IgnoreField<long>("$..talisman_data.talisman_key_id");
}
