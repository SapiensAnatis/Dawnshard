using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class DungeonSkipTest : TestFixture
{
    private const string Endpoint = "/dungeon_skip";

    public DungeonSkipTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task DungeonSkipStart_GrantsRewards()
    {
        int questId = 100010201; // Save the Paladyn (Hard)
        int staminaCost = 8;
        int playCount = 5;

        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        DragaliaResponse<DungeonSkipStartData> response =
            await this.Client.PostMsgpack<DungeonSkipStartData>(
                $"{Endpoint}/start",
                new DungeonSkipStartRequest()
                {
                    party_no = 1,
                    play_count = playCount,
                    support_viewer_id = 1000,
                    quest_id = questId
                }
            );

        response.data.ingame_result_data.reward_record.drop_all.Should().NotBeEmpty();
        response.data.ingame_result_data.reward_record.take_coin.Should().NotBe(0);

        response.data.ingame_result_data.grow_record.take_mana.Should().NotBe(0);
        response.data.ingame_result_data.grow_record.take_player_exp
            .Should()
            .Be(staminaCost * 10 * playCount);

        response.data.ingame_result_data.quest_party_setting_list
            .Should()
            .Contain(x => x.chara_id == Shared.Definitions.Enums.Charas.ThePrince);
        response.data.ingame_result_data.helper_list
            .Should()
            .Contain(x => x.name == "dreadfullydistinct");

        response.data.update_data_list.quest_list
            .Should()
            .Contain(x => x.quest_id == questId && x.play_count == playCount);
        response.data.update_data_list.user_data.stamina_single
            .Should()
            .Be(oldUserData.StaminaSingle - (staminaCost * playCount));
        response.data.update_data_list.user_data.exp
            .Should()
            .Be(oldUserData.Exp + (staminaCost * 10 * playCount));
        response.data.update_data_list.user_data.quest_skip_point
            .Should()
            .Be(oldUserData.QuestSkipPoint - playCount);
    }

    [Fact]
    public async Task DungeonSkipStartAssignUnit_GrantsRewards()
    {
        int questId = 100010301; // Save the Paladyn (Very Hard)
        int staminaCost = 8;
        int playCount = 5;

        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        DragaliaResponse<DungeonSkipStartAssignUnitData> response =
            await this.Client.PostMsgpack<DungeonSkipStartAssignUnitData>(
                $"{Endpoint}/start_assign_unit",
                new DungeonSkipStartAssignUnitRequest()
                {
                    play_count = playCount,
                    support_viewer_id = 1000,
                    quest_id = questId,
                    request_party_setting_list = new List<PartySettingList>()
                    {
                        new() { chara_id = Shared.Definitions.Enums.Charas.ThePrince, }
                    }
                }
            );

        response.data.ingame_result_data.reward_record.drop_all.Should().NotBeEmpty();
        response.data.ingame_result_data.reward_record.take_coin.Should().NotBe(0);

        response.data.ingame_result_data.grow_record.take_mana.Should().NotBe(0);
        response.data.ingame_result_data.grow_record.take_player_exp
            .Should()
            .Be(staminaCost * 10 * playCount);

        response.data.ingame_result_data.quest_party_setting_list
            .Should()
            .Contain(x => x.chara_id == Shared.Definitions.Enums.Charas.ThePrince);
        response.data.ingame_result_data.helper_list
            .Should()
            .Contain(x => x.name == "dreadfullydistinct");

        response.data.update_data_list.quest_list
            .Should()
            .Contain(x => x.quest_id == questId && x.play_count == playCount);
        response.data.update_data_list.user_data.stamina_single
            .Should()
            .Be(oldUserData.StaminaSingle - (staminaCost * playCount));
        response.data.update_data_list.user_data.exp
            .Should()
            .Be(oldUserData.Exp + (staminaCost * 10 * playCount));
        response.data.update_data_list.user_data.quest_skip_point
            .Should()
            .Be(oldUserData.QuestSkipPoint - playCount);
    }

    [Fact]
    public async Task DungeonSkipStartMultipleQuest_GrantsRewards()
    {
        int atpMaster = 201010104; // 9 stam
        int flameRuinsExpert = 202010103; // 9 stam
        int atfMaster = 202060104; // 9 stam
        int brunhildaMaster = 203030104; // 12 stam
        int flameIoStandard = 211010102; // 9 stam

        int totalStamina = 9 + 9 + 9 + 12 + 9;

        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        DragaliaResponse<DungeonSkipStartMultipleQuestData> response =
            await this.Client.PostMsgpack<DungeonSkipStartMultipleQuestData>(
                $"{Endpoint}/start_multiple_quest",
                new DungeonSkipStartMultipleQuestRequest()
                {
                    support_viewer_id = 1000,
                    party_no = 1,
                    request_quest_multiple_list = new List<AtgenRequestQuestMultipleList>
                    {
                        new() { quest_id = atpMaster, play_count = 1, },
                        new() { quest_id = flameRuinsExpert, play_count = 1, },
                        new() { quest_id = atfMaster, play_count = 1, },
                        new() { quest_id = brunhildaMaster, play_count = 1, },
                        new() { quest_id = flameIoStandard, play_count = 1, },
                    }
                }
            );

        response.data.ingame_result_data.reward_record.drop_all.Should().NotBeEmpty();
        response.data.ingame_result_data.reward_record.take_coin.Should().NotBe(0);

        response.data.ingame_result_data.grow_record.take_mana.Should().NotBe(0);
        response.data.ingame_result_data.grow_record.take_player_exp.Should().Be(totalStamina * 10);

        response.data.ingame_result_data.quest_party_setting_list
            .Should()
            .Contain(x => x.chara_id == Shared.Definitions.Enums.Charas.ThePrince);
        response.data.ingame_result_data.helper_list
            .Should()
            .Contain(x => x.name == "dreadfullydistinct");

        response.data.update_data_list.quest_list
            .Select(x => x.quest_id)
            .Should()
            .BeEquivalentTo(
                new List<int>()
                {
                    atpMaster,
                    flameRuinsExpert,
                    atfMaster,
                    brunhildaMaster,
                    flameIoStandard
                }
            );
        response.data.update_data_list.quest_list
            .Should()
            .AllSatisfy(x => x.play_count.Should().Be(1));

        response.data.update_data_list.user_data.stamina_single
            .Should()
            .Be(oldUserData.StaminaSingle - totalStamina);
        response.data.update_data_list.user_data.exp
            .Should()
            .Be(oldUserData.Exp + (totalStamina * 10));
        response.data.update_data_list.user_data.quest_skip_point
            .Should()
            .Be(oldUserData.QuestSkipPoint - 5);
    }

    [Fact]
    public async Task DungeonSkipStart_ReservesCorrectBonusCount()
    {
        int questId = 219011103; // Volk's Wrath: Master (Solo)
        int questEventId = 21900; // QuestData._Gid -> QuestEventGroup._BaseQuestGroupId -> QuestEvent._Id
        int playCount = 5;

        DateTimeOffset resetTime = DateTimeOffset.UtcNow;

        await this.AddToDatabase(
            new DbQuestEvent()
            {
                DeviceAccountId = DeviceAccountId,
                QuestEventId = questEventId,
                LastWeeklyResetTime = resetTime,
                LastDailyResetTime = resetTime,
                WeeklyPlayCount = 2,
                QuestBonusReceiveCount = 2,
            }
        );

        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        DragaliaResponse<DungeonSkipStartData> response =
            await this.Client.PostMsgpack<DungeonSkipStartData>(
                $"{Endpoint}/start",
                new DungeonSkipStartRequest()
                {
                    party_no = 1,
                    play_count = playCount,
                    support_viewer_id = 1000,
                    quest_id = questId
                }
            );

        response.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    weekly_play_count = playCount + 2,
                    daily_play_count = playCount,
                    last_daily_reset_time = resetTime,
                    last_weekly_reset_time = resetTime,
                    quest_bonus_receive_count = 2,
                    quest_bonus_reserve_count = 3,
                    quest_bonus_reserve_time = response.data.ingame_result_data.end_time,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch
                }
            );
    }
}
