using DragaliaAPI.Services.Game;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.FriendController"/>
/// </summary>
public class FriendTest : TestFixture
{
    public FriendTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task GetSupportCharaDetail_GetsCorrectCharacter()
    {
        FriendGetSupportCharaDetailData response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailData>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { support_viewer_id = 1001 }
            )
        ).data;

        response
            .support_user_data_detail.user_support_data.Should()
            .BeEquivalentTo(
                new UserSupportList()
                {
                    viewer_id = 1001,
                    name = "Nightmerp",
                    level = 250,
                    emblem_id = (Emblems)10250305,
                    max_party_power = 9999,
                    support_chara = new()
                    {
                        chara_id = Charas.GalaEmile,
                        level = 80,
                        additional_max_level = 0,
                        rarity = 5,
                        hp = 789,
                        attack = 486,
                        hp_plus_count = 100,
                        attack_plus_count = 100,
                        ability_1_level = 2,
                        ability_2_level = 2,
                        ability_3_level = 2,
                        ex_ability_level = 5,
                        ex_ability_2_level = 5,
                        skill_1_level = 3,
                        skill_2_level = 2,
                        is_unlock_edit_skill = true
                    },
                    support_dragon = new()
                    {
                        dragon_key_id = 0,
                        dragon_id = Dragons.GalaBahamut,
                        level = 100,
                        hp = 368,
                        attack = 128,
                        skill_1_level = 2,
                        ability_1_level = 5,
                        ability_2_level = 5,
                        hp_plus_count = 50,
                        attack_plus_count = 50,
                        limit_break_count = 4
                    },
                    support_weapon_body = new()
                    {
                        weapon_body_id = WeaponBodies.AqueousPrison,
                        buildup_count = 80,
                        limit_break_count = 8,
                        limit_over_count = 1,
                        equipable_count = 4,
                        additional_crest_slot_type_1_count = 1,
                        additional_crest_slot_type_2_count = 0,
                        additional_crest_slot_type_3_count = 2
                    },
                    support_talisman = new()
                    {
                        talisman_key_id = 0,
                        talisman_id = Talismans.GalaEmile,
                        additional_attack = 100,
                        additional_hp = 100
                    },
                    support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            ability_crest_id = AbilityCrests.ARoyalTeaParty,
                            buildup_count = 50,
                            limit_break_count = 4,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            equipable_count = 4
                        },
                        new()
                        {
                            ability_crest_id = AbilityCrests.QueenoftheBlueSeas,
                            buildup_count = 50,
                            limit_break_count = 4,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            equipable_count = 4
                        },
                        new()
                        {
                            ability_crest_id = AbilityCrests.PeacefulWaterfront,
                            buildup_count = 50,
                            limit_break_count = 4,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            equipable_count = 4
                        },
                    },
                    support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            ability_crest_id = AbilityCrests.HisCleverBrother,
                            buildup_count = 40,
                            limit_break_count = 4,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            equipable_count = 4
                        },
                        new()
                        {
                            ability_crest_id = AbilityCrests.DragonsNest,
                            buildup_count = 20,
                            limit_break_count = 4,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            equipable_count = 4
                        },
                    },
                    support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                            buildup_count = 30,
                            limit_break_count = 4,
                            hp_plus_count = 40,
                            attack_plus_count = 40,
                            equipable_count = 4
                        },
                        new()
                        {
                            ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                            buildup_count = 30,
                            limit_break_count = 4,
                            hp_plus_count = 40,
                            attack_plus_count = 40,
                            equipable_count = 4
                        }
                    },
                    guild = new() { guild_id = 0, guild_name = "Guild" }
                },
                o => o.Excluding(x => x.last_login_date)
            );

        response.support_user_data_detail.is_friend.Should().BeTrue();
    }

    [Fact]
    public async Task GetSupportCharaDetail_GetsCorrectDefaultCharacter()
    {
        FriendGetSupportCharaDetailData response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailData>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { support_viewer_id = 0 }
            )
        ).data;

        response
            .support_user_data_detail.user_support_data.Should()
            .BeEquivalentTo(HelperService.StubData.SupportListData.support_user_list.First());

        response.support_user_data_detail.is_friend.Should().BeFalse();
    }
}
