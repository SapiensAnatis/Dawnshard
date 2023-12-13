using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Missions;

public class MissionTest : TestFixture
{
    public MissionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.ApiContext.PlayerMissions.ExecuteDelete();
        this.ApiContext.CompletedDailyMissions.ExecuteDelete();
        this.ApiContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task UnlockDrillMissionGroup_ValidRequest_UnlocksGroup()
    {
        DragaliaResponse<MissionUnlockDrillMissionGroupData> resp =
            await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupData>(
                "mission/unlock_drill_mission_group",
                new MissionUnlockDrillMissionGroupRequest(1)
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data
            .drill_mission_list
            .Should()
            .HaveCount(55)
            .And
            .ContainEquivalentOf(
                new DrillMissionList(
                    100100,
                    0,
                    0,
                    DateTimeOffset.UnixEpoch,
                    DateTimeOffset.UnixEpoch
                )
            );
    }

    [Fact]
    public async Task UnlockMainStoryGroup_ValidRequest_UnlocksGroup()
    {
        DragaliaResponse<MissionUnlockMainStoryGroupData> resp =
            await this.Client.PostMsgpack<MissionUnlockMainStoryGroupData>(
                "mission/unlock_main_story_group",
                new MissionUnlockMainStoryGroupRequest(1)
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.main_story_mission_list.Should().HaveCount(5);
        // Don't test for a specific quest as other tests mess with the quest progress
    }

    [Fact]
    public async Task DrillMission_ReadStory_CompletesMission()
    {
        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupData>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1)
        );

        DragaliaResponse<QuestReadStoryData> resp =
            await this.Client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1000106 }
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.update_data_list.mission_notice.drill_mission_notice.is_update.Should().Be(1);
        resp.data
            .update_data_list
            .mission_notice
            .drill_mission_notice
            .completed_mission_count
            .Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.data
            .update_data_list
            .mission_notice
            .drill_mission_notice
            .new_complete_mission_id_list
            .Should()
            .Contain(100200);

        DragaliaResponse<MissionReceiveDrillRewardData> rewardResp =
            await this.Client.PostMsgpack<MissionReceiveDrillRewardData>(
                "/mission/receive_drill_reward",
                new MissionReceiveDrillRewardRequest(new[] { 100200 }, Enumerable.Empty<int>())
            );

        rewardResp.data_headers.result_code.Should().Be(ResultCode.Success);
        rewardResp.data.entity_result.converted_entity_list.Should().NotBeNull();
        rewardResp.data.drill_mission_list.Should().HaveCount(55);
    }

    [Fact]
    public async Task DrillMission_TreasureTrade_CompletesMission()
    {
        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupData>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(3)
        );

        DragaliaResponse<TreasureTradeTradeData> resp =
            await this.Client.PostMsgpack<TreasureTradeTradeData>(
                "/treasure_trade/trade",
                new TreasureTradeTradeRequest() { treasure_trade_id = 10020101, trade_count = 1 }
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.update_data_list.mission_notice.drill_mission_notice.is_update.Should().Be(1);
        resp.data
            .update_data_list
            .mission_notice
            .drill_mission_notice
            .completed_mission_count
            .Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.data
            .update_data_list
            .mission_notice
            .drill_mission_notice
            .new_complete_mission_id_list
            .Should()
            .Contain(300100);
    }

    [Fact]
    public async Task DrillMission_WyrmprintBuildup_CompletesMission()
    {
        await this.AddToDatabase(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.Aromatherapy,
                LimitBreakCount = 4,
            }
        );

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupData>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(3)
        );

        DragaliaResponse<AbilityCrestBuildupPieceData> resp =
            await this.Client.PostMsgpack<AbilityCrestBuildupPieceData>(
                "/ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.Aromatherapy,
                    buildup_ability_crest_piece_list = Enumerable
                        .Range(2, 15)
                        .Select(
                            x =>
                                new AtgenBuildupAbilityCrestPieceList()
                                {
                                    buildup_piece_type = BuildupPieceTypes.Stats,
                                    step = x
                                }
                        )
                }
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.update_data_list.mission_notice.drill_mission_notice.is_update.Should().Be(1);
        resp.data
            .update_data_list
            .mission_notice
            .drill_mission_notice
            .completed_mission_count
            .Should()
            .BeGreaterThan(1);
        resp.data
            .update_data_list
            .mission_notice
            .drill_mission_notice
            .new_complete_mission_id_list
            .Should()
            .Contain(301700);
    }

    [Fact]
    public async Task ReceiveReward_Wyrmprint_DoesNotGive0Copies()
    {
        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Id = 10220101,
                Type = MissionType.MemoryEvent,
                ViewerId = ViewerId,
                Progress = 1,
                State = MissionState.Completed,
            }
        );

        MissionReceiveMemoryEventRewardData response = (
            await this.Client.PostMsgpack<MissionReceiveMemoryEventRewardData>(
                "mission/receive_memory_event_reward",
                new MissionReceiveMemoryEventRewardRequest()
                {
                    memory_event_mission_id_list = new[] { 10220101 }, // Participate in the Event (Toll of the Deep)
                }
            )
        ).data;

        response
            .update_data_list
            .ability_crest_list
            .Should()
            .Contain(
                x => x.ability_crest_id == AbilityCrests.HavingaSummerBall && x.equipable_count == 1
            );
    }

    [Fact]
    public async Task ReceiveReward_Daily_ClaimsReward()
    {
        int missionId1 = 15070301; // Clear a Quest
        int missionId2 = 15070401; // Clear Three Quests

        DateOnly today = new(2023, 12, 13);
        DateOnly yesterday = new(2023, 12, 12);

        await this.AddToDatabase(
            [
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId1,
                    Date = today,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Date = today,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId1,
                    Date = yesterday,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Date = yesterday,
                }
            ]
        );

        await this.AddToDatabase(
            [
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId1,
                    Type = MissionType.Daily,
                    State = MissionState.Completed,
                },
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Type = MissionType.Daily,
                    State = MissionState.Completed,
                }
            ]
        );

        DragaliaResponse<MissionReceiveDailyRewardData> response =
            await this.Client.PostMsgpack<MissionReceiveDailyRewardData>(
                "mission/receive_daily_reward",
                new MissionReceiveDailyRewardRequest()
                {
                    mission_params_list =
                    [
                        new() { daily_mission_id = missionId1, day_no = today, },
                        new() { daily_mission_id = missionId2, day_no = today, },
                        new() { daily_mission_id = missionId1, day_no = yesterday, },
                    ]
                }
            );

        response
            .data
            .daily_mission_list
            .Should()
            .BeEquivalentTo(
                [
                    new DailyMissionList()
                    {
                        daily_mission_id = missionId1,
                        day_no = today,
                        state = MissionState.Claimed,
                    },
                    new DailyMissionList()
                    {
                        daily_mission_id = missionId2,
                        day_no = today,
                        state = MissionState.Claimed,
                    },
                    new DailyMissionList()
                    {
                        daily_mission_id = missionId2,
                        day_no = yesterday,
                        state = MissionState.Completed,
                    },
                ],
                opts =>
                    opts.Including(x => x.daily_mission_id)
                        .Including(x => x.day_no)
                        .Including(x => x.state)
            );
    }

    [Fact]
    public async Task GetDailyMissionList_ReturnsUnionOfTables()
    {
        int missionId = 15070301; // Clear a Quest
        DateOnly today = new(2023, 12, 13);
        DateOnly yesterday = new(2023, 12, 12);

        await this.AddToDatabase(
            [
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId,
                    Date = today,
                },
                new DbCompletedDailyMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId,
                    Date = yesterday,
                },
            ]
        );

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                ViewerId = this.ViewerId,
                Id = missionId,
                Type = MissionType.Daily,
                State = MissionState.Completed,
            }
        );

        DragaliaResponse<MissionGetMissionListData> response =
            await this.Client.PostMsgpack<MissionGetMissionListData>(
                "mission/get_mission_list",
                new MissionGetMissionListRequest()
            );

        response
            .data
            .daily_mission_list
            .Should()
            .BeEquivalentTo(
                [
                    new DailyMissionList()
                    {
                        daily_mission_id = missionId,
                        day_no = today,
                        state = MissionState.Completed
                    },
                    new DailyMissionList()
                    {
                        daily_mission_id = missionId,
                        day_no = yesterday,
                        state = MissionState.Completed
                    }
                ],
                opts =>
                    opts.Including(x => x.daily_mission_id)
                        .Including(x => x.day_no)
                        .Including(x => x.state)
            );
    }
}
