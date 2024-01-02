using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Helpers;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Missions;

public class MissionTest : TestFixture
{
    public MissionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task UnlockDrillMissionGroup_ValidRequest_UnlocksGroup()
    {
        DragaliaResponse<MissionUnlockDrillMissionGroupData> resp =
            await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupData>(
                "mission/unlock_drill_mission_group",
                new MissionUnlockDrillMissionGroupRequest(1)
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.drill_mission_list.Should()
            .HaveCount(55)
            .And.ContainEquivalentOf(
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
        resp.data.update_data_list.mission_notice.drill_mission_notice.completed_mission_count.Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.data.update_data_list.mission_notice.drill_mission_notice.new_complete_mission_id_list.Should()
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
        resp.data.update_data_list.mission_notice.drill_mission_notice.completed_mission_count.Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.data.update_data_list.mission_notice.drill_mission_notice.new_complete_mission_id_list.Should()
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
        resp.data.update_data_list.mission_notice.drill_mission_notice.completed_mission_count.Should()
            .BeGreaterThan(1);
        resp.data.update_data_list.mission_notice.drill_mission_notice.new_complete_mission_id_list.Should()
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
            .update_data_list.ability_crest_list.Should()
            .Contain(
                x => x.ability_crest_id == AbilityCrests.HavingaSummerBall && x.equipable_count == 1
            );
    }

    [Fact]
    public async Task ReceiveReward_Daily_ClaimsReward()
    {
        int missionId1 = 15070301; // Clear a Quest
        int missionId2 = 15070401; // Clear Three Quests

        ResetHelper resetHelper = new(TimeProvider.System);

        DateOnly today = DateOnly.FromDateTime(resetHelper.LastDailyReset.Date);
        DateOnly yesterday = today.AddDays(-1);

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
                    Start = resetHelper.LastDailyReset,
                    End = resetHelper.LastDailyReset.AddDays(1)
                },
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId2,
                    Type = MissionType.Daily,
                    State = MissionState.Completed,
                    Start = resetHelper.LastDailyReset,
                    End = resetHelper.LastDailyReset.AddDays(1)
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
            .data.daily_mission_list.Should()
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
    public async Task ReceiveReward_Stamp_GrantsWyrmite()
    {
        // The Miracle Of Dragonyule: Clear a Boss Battle Five Times. Grants 'Splendid!' sticker
        int missionId = 10020502;

        int oldWyrmite = this.ApiContext.PlayerUserData.First(
            x => x.ViewerId == this.ViewerId
        ).Crystal;

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Id = missionId,
                Type = MissionType.MemoryEvent,
                State = MissionState.Completed,
                Progress = 5
            }
        );

        MissionReceiveMemoryEventRewardData response = (
            await this.Client.PostMsgpack<MissionReceiveMemoryEventRewardData>(
                "mission/receive_memory_event_reward",
                new MissionReceiveMemoryEventRewardRequest()
                {
                    memory_event_mission_id_list = [missionId],
                }
            )
        ).data;

        response.update_data_list.user_data.crystal.Should().Be(oldWyrmite + 25);
    }

    [Fact]
    public async Task GetDailyMissionList_ReturnsUnionOfTables()
    {
        int missionId = 15070301; // Clear a Quest
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly yesterday = today.AddDays(-1);

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
            .data.daily_mission_list.Should()
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

    [Fact]
    public async Task ClearDaily_ExistingClearInDb_DoesNotThrow()
    {
        int missionId = 15070101; // Perform an Item Summon
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);

        await this.AddToDatabase(
            new DbCompletedDailyMission()
            {
                ViewerId = this.ViewerId,
                Id = missionId,
                Progress = 1,
                Date = date,
            }
        );

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                ViewerId = this.ViewerId,
                Id = missionId,
                Type = MissionType.Daily,
                Progress = 0,
                State = MissionState.InProgress,
            }
        );

        DragaliaResponse<ShopItemSummonExecData> response =
            await this.Client.PostMsgpack<ShopItemSummonExecData>(
                "shop/item_summon_exec",
                new ShopItemSummonExecRequest() { payment_type = PaymentTypes.Wyrmite },
                ensureSuccessHeader: false
            );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task GetDrillMissionList_ReturnsCompletedGroups()
    {
        DbPlayerMission ToDbMission(DrillMission mission)
        {
            return new()
            {
                ViewerId = this.ViewerId,
                Id = mission.Id,
                State = MissionState.Claimed,
                Type = MissionType.Drill
            };
        }

        MissionGetDrillMissionListData response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListData>(
                "mission/get_drill_mission_list",
                new MissionGetDrillMissionListRequest()
            )
        ).data;

        response.drill_mission_group_list.Should().BeEmpty();

        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 1)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListData>(
                "mission/get_drill_mission_list",
                new MissionGetDrillMissionListRequest()
            )
        ).data;

        response.drill_mission_group_list.Should().BeEquivalentTo([new DrillMissionGroupList(1)]);

        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 2)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListData>(
                "mission/get_drill_mission_list",
                new MissionGetDrillMissionListRequest()
            )
        ).data;

        response
            .drill_mission_group_list.Should()
            .BeEquivalentTo([new DrillMissionGroupList(1), new DrillMissionGroupList(2)]);

        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 3)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListData>(
                "mission/get_drill_mission_list",
                new MissionGetDrillMissionListRequest()
            )
        ).data;

        response
            .drill_mission_group_list.Should()
            .BeEquivalentTo(
                [
                    new DrillMissionGroupList(1),
                    new DrillMissionGroupList(2),
                    new DrillMissionGroupList(3)
                ]
            );
    }

    [Fact]
    public async Task GetMissionList_InBetweenDrillGroups_ReturnsRewardCount1()
    {
        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 1)
                .Select(
                    x =>
                        new DbPlayerMission()
                        {
                            ViewerId = this.ViewerId,
                            Id = x.Id,
                            State = MissionState.Claimed,
                            Type = MissionType.Drill
                        }
                )
        );

        MissionGetMissionListData response = (
            await this.Client.PostMsgpack<MissionGetMissionListData>(
                "mission/get_mission_list",
                new MissionGetMissionListRequest()
            )
        ).data;

        response
            .mission_notice.drill_mission_notice.receivable_reward_count.Should()
            .Be(1, "because otherwise the drill mission popup disappears");
    }

    [Fact]
    public async Task GetMissionList_DoesNotReturnOutOfDateMissions()
    {
        DbPlayerMission expiredMission =
            new()
            {
                Id = 11650101,
                Type = MissionType.Period,
                State = MissionState.InProgress,
                Start = DateTimeOffset.UtcNow.AddDays(-2),
                End = DateTimeOffset.UtcNow.AddDays(-1),
            };
        DbPlayerMission notStartedMission =
            new()
            {
                Id = 11650201,
                Type = MissionType.Period,
                State = MissionState.InProgress,
                Start = DateTimeOffset.UtcNow.AddDays(+1),
                End = DateTimeOffset.UtcNow.AddDays(+2),
            };
        DbPlayerMission expectedMission =
            new()
            {
                Id = 11650301,
                Type = MissionType.Period,
                State = MissionState.InProgress,
                Start = DateTimeOffset.UtcNow.AddDays(-1),
                End = DateTimeOffset.UtcNow.AddDays(+1),
            };
        DbPlayerMission otherExpectedMission =
            new()
            {
                Id = 11650302,
                Type = MissionType.Period,
                State = MissionState.InProgress,
                Start = DateTimeOffset.UnixEpoch,
                End = DateTimeOffset.UnixEpoch,
            };

        await this.AddRangeToDatabase(
            [expiredMission, notStartedMission, expectedMission, otherExpectedMission]
        );

        MissionGetMissionListData response = (
            await this.Client.PostMsgpack<MissionGetMissionListData>(
                "mission/get_mission_list",
                new MissionGetMissionListRequest()
            )
        ).data;

        response
            .period_mission_list.Should()
            .HaveCount(2)
            .And.Contain(x => x.period_mission_id == expectedMission.Id)
            .And.Contain(x => x.period_mission_id == otherExpectedMission.Id);
    }
}
