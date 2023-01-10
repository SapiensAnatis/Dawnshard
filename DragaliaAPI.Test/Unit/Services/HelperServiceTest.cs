using AutoMapper;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Unit.Services;

public class HelperServiceTest
{
    private readonly IHelperService helperService;
    private readonly IMapper mapper;

    public HelperServiceTest()
    {
        this.mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();

        this.helperService = new HelperService(this.mapper);
    }

    [Fact]
    public void BuildHelperDataContainsCorrectInformationWhenFriended()
    {
        UserSupportList? helperInfo = StubData.HelperList.support_user_list
            .Where(helper => helper.viewer_id == 1000)
            .FirstOrDefault(defaultValue: null);

        AtgenSupportUserDetailList? helperDetails = StubData.HelperList.support_user_detail_list
            .Where(helper => helper.viewer_id == 1000)
            .FirstOrDefault(defaultValue: null);

        AtgenSupportData supportData = this.helperService.BuildHelperData(
            helperInfo!,
            helperDetails!
        );

        supportData.viewer_id.Should().Be(1000);
        supportData.name.Should().BeEquivalentTo("Euden");
        supportData.is_friend.Should().Be(1);
        supportData.chara_data.Should().BeOfType<CharaList>();
        supportData.chara_data.chara_id.Should().Be(Charas.ThePrince);
        supportData.dragon_data.Should().BeOfType<DragonList>();
        supportData.weapon_body_data.Should().BeOfType<GameWeaponBody>();
        supportData.crest_slot_type_1_crest_list
            .Should()
            .BeEquivalentTo(
                new List<GameAbilityCrest>()
                {
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                }
            );
        supportData.crest_slot_type_2_crest_list
            .Should()
            .BeEquivalentTo(
                new List<GameAbilityCrest>()
                {
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                }
            );
        supportData.crest_slot_type_3_crest_list
            .Should()
            .BeEquivalentTo(
                new List<GameAbilityCrest>()
                {
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                }
            );
    }

    [Fact]
    public void BuildHelperDataContainsCorrectInformationWhenNotFriended()
    {
        UserSupportList? helperInfo = StubData.HelperList.support_user_list
            .Where(helper => helper.viewer_id == 1001)
            .FirstOrDefault(defaultValue: null);

        AtgenSupportUserDetailList? helperDetails = StubData.HelperList.support_user_detail_list
            .Where(helper => helper.viewer_id == 1001)
            .FirstOrDefault(defaultValue: null);

        AtgenSupportData supportData = this.helperService.BuildHelperData(
            helperInfo!,
            helperDetails!
        );

        supportData.viewer_id.Should().Be(1001);
        supportData.name.Should().BeEquivalentTo("Elisanne");
        supportData.is_friend.Should().Be(0);
        supportData.chara_data.Should().BeOfType<CharaList>();
        supportData.chara_data.chara_id.Should().Be(Charas.Elisanne);
        supportData.dragon_data.Should().BeOfType<DragonList>();
        supportData.weapon_body_data.Should().BeOfType<GameWeaponBody>();
        supportData.crest_slot_type_1_crest_list
            .Should()
            .BeEquivalentTo(
                new List<GameAbilityCrest>()
                {
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                }
            );
        supportData.crest_slot_type_2_crest_list
            .Should()
            .BeEquivalentTo(
                new List<GameAbilityCrest>()
                {
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                }
            );
        supportData.crest_slot_type_3_crest_list
            .Should()
            .BeEquivalentTo(
                new List<GameAbilityCrest>()
                {
                    new() { ability_crest_id = 0 },
                    new() { ability_crest_id = 0 },
                }
            );
    }

    private static class StubData
    {
        public static readonly QuestGetSupportUserListData HelperList =
            new()
            {
                support_user_list = new List<UserSupportList>()
                {
                    new()
                    {
                        viewer_id = 1000,
                        name = "Euden",
                        level = 10,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 40000002,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.ThePrince,
                            level = 10,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 60,
                            attack = 40,
                            hp_plus_count = 0,
                            attack_plus_count = 0,
                            ability_1_level = 0,
                            ability_2_level = 0,
                            ability_3_level = 0,
                            ex_ability_level = 1,
                            ex_ability_2_level = 1,
                            skill_1_level = 1,
                            skill_2_level = 0,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new() { dragon_key_id = 0, },
                        support_weapon_body = new() { weapon_body_id = 0, },
                        support_talisman = new() { talisman_key_id = 0, },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1001,
                        name = "Elisanne",
                        level = 10,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 40000002,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Elisanne,
                            level = 10,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 60,
                            attack = 40,
                            hp_plus_count = 0,
                            attack_plus_count = 0,
                            ability_1_level = 0,
                            ability_2_level = 0,
                            ability_3_level = 0,
                            ex_ability_level = 1,
                            ex_ability_2_level = 1,
                            skill_1_level = 1,
                            skill_2_level = 0,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new() { dragon_key_id = 0, },
                        support_weapon_body = new() { weapon_body_id = 0, },
                        support_talisman = new() { talisman_key_id = 0, },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new() { ability_crest_id = 0 },
                            new() { ability_crest_id = 0 },
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                },
                support_user_detail_list = new List<AtgenSupportUserDetailList>()
                {
                    new() { viewer_id = 1000, is_friend = 1, },
                    new() { viewer_id = 1001, is_friend = 0, }
                }
            };
    }
}
