using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Test.Unit.Controllers;

public class FriendControllerTest
{
    private readonly FriendController friendController;
    private readonly Mock<IHelperService> mockHelperService;

    public FriendControllerTest()
    {
        this.mockHelperService = new(MockBehavior.Strict);

        this.friendController = new FriendController(mockHelperService.Object);
    }

    [Fact]
    public async Task GetSupportCharaDetailContainsCorrectInformationWhenFound()
    {
        this.mockHelperService
            .Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListData()
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
                            support_crest_slot_type_1_list =
                                new List<AtgenSupportCrestSlotType1List>()
                                {
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                },
                            support_crest_slot_type_2_list =
                                new List<AtgenSupportCrestSlotType1List>()
                                {
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                },
                            support_crest_slot_type_3_list =
                                new List<AtgenSupportCrestSlotType1List>()
                                {
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                },
                            guild = new() { guild_id = 0, guild_name = "Guild" }
                        }
                    },
                    support_user_detail_list = new List<AtgenSupportUserDetailList>()
                    {
                        new() { viewer_id = 1000, is_friend = true, },
                    }
                }
            );

        ActionResult<DragaliaResponse<object>> response =
            await this.friendController.GetSupportCharaDetail(
                new FriendGetSupportCharaDetailRequest() { support_viewer_id = 1000 }
            );

        FriendGetSupportCharaDetailData? data = response.GetData<FriendGetSupportCharaDetailData>();
        data.Should().NotBeNull();

        data!.support_user_data_detail.user_support_data.viewer_id.Should().Be(1000);
        data!.support_user_data_detail.user_support_data.support_chara
            .Should()
            .BeEquivalentTo(
                new AtgenSupportChara()
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
                }
            );
        data!.support_user_data_detail.is_friend.Should().Be(true);

        this.mockHelperService.VerifyAll();
    }

    [Fact]
    public async Task GetSupportCharaDetailContainsDefaultInformationWhenNotFound()
    {
        this.mockHelperService
            .Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListData()
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
                            support_crest_slot_type_1_list =
                                new List<AtgenSupportCrestSlotType1List>()
                                {
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                },
                            support_crest_slot_type_2_list =
                                new List<AtgenSupportCrestSlotType1List>()
                                {
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                },
                            support_crest_slot_type_3_list =
                                new List<AtgenSupportCrestSlotType1List>()
                                {
                                    new() { ability_crest_id = 0 },
                                    new() { ability_crest_id = 0 },
                                },
                            guild = new() { guild_id = 0, guild_name = "Guild" }
                        }
                    },
                    support_user_detail_list = new List<AtgenSupportUserDetailList>()
                    {
                        new() { viewer_id = 1000, is_friend = true },
                    }
                }
            );

        ActionResult<DragaliaResponse<object>> response =
            await this.friendController.GetSupportCharaDetail(
                new FriendGetSupportCharaDetailRequest() { support_viewer_id = 0 }
            );

        FriendGetSupportCharaDetailData? data = response.GetData<FriendGetSupportCharaDetailData>();
        data.Should().NotBeNull();

        data!.support_user_data_detail.user_support_data
            .Should()
            .BeEquivalentTo(
                new UserSupportList() { support_chara = new() { chara_id = Charas.ThePrince } }
            );

        data!.support_user_data_detail.is_friend.Should().Be(false);

        this.mockHelperService.VerifyAll();
    }
}
