using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Integration.Test.Features;

public class QuestBonusTest : TestFixture
{
    /*
     * Use this query on the SQLite DB to find quest event IDs
     * SELECT q._Id as QuestId, qe._Id as EventId, t._Text as QuestName FROM QuestData q
     * JOIN QuestEventGroup qeg ON q._Gid = qeg._Id
     * JOIN QuestEvent qe ON qe._Id = qeg._BaseQuestGroupId
     * JOIN TextLabel t ON _QuestViewName = t._Id
     */

    public QuestBonusTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task QuestBonus_CanClaimWeeklyAgitoBonus()
    {
        int questId = 219041102; // Ayaha and Otoha's Wrath: Expert (Solo)
        int questEventId = 21900;

        DragaliaResponse<DungeonRecordRecordData> response = await this.CompleteQuest(questId);

        response.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    quest_bonus_receive_count = 0,
                    quest_bonus_reserve_count = 1,
                    quest_bonus_reserve_time = response.data.ingame_result_data.end_time,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch,
                    last_daily_reset_time = response.data.ingame_result_data.end_time,
                    last_weekly_reset_time = response.data.ingame_result_data.end_time,
                    daily_play_count = 1,
                    weekly_play_count = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusData> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusData>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    quest_event_id = questEventId,
                    is_receive = true,
                    receive_bonus_count = 1
                }
            );

        bonusResponse.data.receive_quest_bonus.target_quest_id.Should().Be(questId);
        bonusResponse.data.receive_quest_bonus.receive_bonus_count.Should().Be(1);
        bonusResponse.data.receive_quest_bonus.bonus_factor.Should().Be(1);

        bonusResponse.data.receive_quest_bonus.quest_bonus_entity_list
            .Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        entity_type = EntityTypes.Material,
                        entity_id = (int)Materials.ConsecratedWater,
                    },
                    new()
                    {
                        entity_type = EntityTypes.Material,
                        entity_id = (int)Materials.SoaringOnesMaskFragment,
                    },
                    new()
                    {
                        entity_type = EntityTypes.Material,
                        entity_id = (int)Materials.LiberatedOnesMaskFragment,
                    },
                    new()
                    {
                        entity_type = EntityTypes.Material,
                        entity_id = (int)Materials.RebelliousOnesCruelty,
                    },
                    new()
                    {
                        entity_type = EntityTypes.Material,
                        entity_id = (int)Materials.TwinklingSand,
                    }
                },
                opts => opts.Excluding(x => x.entity_quantity)
            );

        bonusResponse.data.update_data_list.material_list.Should().NotBeEmpty();
        bonusResponse.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    quest_bonus_receive_count = 1,
                    quest_bonus_reserve_count = 0,
                    quest_bonus_reserve_time = DateTimeOffset.UnixEpoch,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch,
                    last_daily_reset_time = response.data.ingame_result_data.end_time,
                    last_weekly_reset_time = response.data.ingame_result_data.end_time,
                    daily_play_count = 1,
                    weekly_play_count = 1
                }
            );
    }

    [Fact]
    public async Task QuestBonus_AllClaimed_NoneReserved()
    {
        int questId = 201010104; // Avenue to Power: Master
        int questEventId = 20101;

        DateTimeOffset resetTime = DateTimeOffset.UtcNow;

        await this.AddToDatabase(
            new DbQuestEvent()
            {
                DeviceAccountId = DeviceAccountId,
                QuestEventId = questEventId,
                LastWeeklyResetTime = resetTime,
                LastDailyResetTime = resetTime,
                DailyPlayCount = 10,
                WeeklyPlayCount = 20,
                QuestBonusReceiveCount = 1,
            }
        );

        DragaliaResponse<DungeonRecordRecordData> response = await this.CompleteQuest(questId);

        response.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    quest_bonus_receive_count = 1,
                    quest_bonus_reserve_count = 0,
                    quest_bonus_reserve_time = DateTimeOffset.UnixEpoch,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch,
                    last_daily_reset_time = resetTime,
                    last_weekly_reset_time = resetTime,
                    daily_play_count = 11,
                    weekly_play_count = 21
                }
            );
    }

    [Fact]
    public async Task QuestBonus_AllClaimedLastWeek_CanReceiveAgain()
    {
        int questId = 225030101; // Ciella's Wrath: Legend (Co-op)
        int questEventId = 22500;

        await this.AddToDatabase(
            new DbQuestEvent()
            {
                DeviceAccountId = DeviceAccountId,
                QuestEventId = questEventId,
                LastWeeklyResetTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(7),
                LastDailyResetTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(7),
                WeeklyPlayCount = 7,
                QuestBonusReceiveCount = 5,
            }
        );

        DragaliaResponse<DungeonRecordRecordData> response = await this.CompleteQuest(questId);

        response.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    quest_bonus_receive_count = 0,
                    quest_bonus_reserve_count = 1,
                    quest_bonus_reserve_time = response.data.ingame_result_data.end_time,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch,
                    last_daily_reset_time = response.data.ingame_result_data.end_time,
                    last_weekly_reset_time = response.data.ingame_result_data.end_time,
                    daily_play_count = 1,
                    weekly_play_count = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusData> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusData>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    quest_event_id = questEventId,
                    is_receive = true,
                    receive_bonus_count = 1
                }
            );

        bonusResponse.data.receive_quest_bonus.target_quest_id.Should().Be(questId);
        bonusResponse.data.receive_quest_bonus.receive_bonus_count.Should().Be(1);
        bonusResponse.data.receive_quest_bonus.bonus_factor.Should().Be(1);

        bonusResponse.data.receive_quest_bonus.quest_bonus_entity_list.Should().NotBeEmpty();
        bonusResponse.data.update_data_list.material_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task QuestBonus_NotClaiming_SetsReserveCountToZero()
    {
        int questId = 210020104; // High Mercury's Trial: Master
        int questEventId = 21000;

        DragaliaResponse<DungeonRecordRecordData> response = await this.CompleteQuest(questId);

        response.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    quest_bonus_receive_count = 0,
                    quest_bonus_reserve_count = 1,
                    quest_bonus_reserve_time = response.data.ingame_result_data.end_time,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch,
                    last_daily_reset_time = response.data.ingame_result_data.end_time,
                    last_weekly_reset_time = response.data.ingame_result_data.end_time,
                    daily_play_count = 1,
                    weekly_play_count = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusData> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusData>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    quest_event_id = questEventId,
                    is_receive = false,
                    receive_bonus_count = 0
                }
            );

        bonusResponse.data.receive_quest_bonus.target_quest_id.Should().Be(questId);
        bonusResponse.data.receive_quest_bonus.receive_bonus_count.Should().Be(0);

        bonusResponse.data.update_data_list.material_list.Should().BeNullOrEmpty();
        bonusResponse.data.update_data_list.quest_event_list
            .Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    quest_event_id = questEventId,
                    quest_bonus_receive_count = 0,
                    quest_bonus_reserve_count = 0,
                    quest_bonus_reserve_time = DateTimeOffset.UnixEpoch,
                    quest_bonus_stack_count = 0,
                    quest_bonus_stack_time = DateTimeOffset.UnixEpoch,
                    last_daily_reset_time = response.data.ingame_result_data.end_time,
                    last_weekly_reset_time = response.data.ingame_result_data.end_time,
                    daily_play_count = 1,
                    weekly_play_count = 1
                }
            );
    }

    private async Task<DragaliaResponse<DungeonRecordRecordData>> CompleteQuest(int questId)
    {
        DragaliaResponse<DungeonStartStartData> startResponse =
            await this.Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    quest_id = questId,
                    party_no_list = new() { 1 }
                }
            );

        DragaliaResponse<DungeonRecordRecordData> recordResponse =
            await this.Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse.data.ingame_data.dungeon_key,
                    play_record = new()
                    {
                        time = 10,
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord()
                    }
                }
            );

        return recordResponse;
    }
}
