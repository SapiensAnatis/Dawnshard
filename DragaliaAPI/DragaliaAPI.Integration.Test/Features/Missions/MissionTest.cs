using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Helpers;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Integration.Test.Features.Missions;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class MissionTest : TestFixture
{
    public MissionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task UnlockDrillMissionGroup_ValidRequest_UnlocksGroup()
    {
        DragaliaResponse<MissionUnlockDrillMissionGroupResponse> resp =
            await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
                "mission/unlock_drill_mission_group",
                new MissionUnlockDrillMissionGroupRequest(1)
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.DrillMissionList.Should()
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
        DragaliaResponse<MissionUnlockMainStoryGroupResponse> resp =
            await this.Client.PostMsgpack<MissionUnlockMainStoryGroupResponse>(
                "mission/unlock_main_story_group",
                new MissionUnlockMainStoryGroupRequest(1)
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.MainStoryMissionList.Should().HaveCount(5);
        // Don't test for a specific quest as other tests mess with the quest progress
    }

    [Fact]
    public async Task DrillMission_ReadStory_CompletesMission()
    {
        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1)
        );

        DragaliaResponse<QuestReadStoryResponse> resp =
            await this.Client.PostMsgpack<QuestReadStoryResponse>(
                "/quest/read_story",
                new QuestReadStoryRequest() { QuestStoryId = 1000106 }
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(100200);

        DragaliaResponse<MissionReceiveDrillRewardResponse> rewardResp =
            await this.Client.PostMsgpack<MissionReceiveDrillRewardResponse>(
                "/mission/receive_drill_reward",
                new MissionReceiveDrillRewardRequest(new[] { 100200 }, Enumerable.Empty<int>())
            );

        rewardResp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        rewardResp.Data.EntityResult.ConvertedEntityList.Should().NotBeNull();
        rewardResp.Data.DrillMissionList.Should().HaveCount(55);
    }

    [Fact]
    public async Task DrillMission_TreasureTrade_CompletesMission()
    {
        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(3)
        );

        DragaliaResponse<TreasureTradeTradeResponse> resp =
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "/treasure_trade/trade",
                new TreasureTradeTradeRequest() { TreasureTradeId = 10020101, TradeCount = 1 }
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1); // One has to be completed because of the above, multiple can be completed due to other factors
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
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

        await this.Client.PostMsgpack<MissionUnlockDrillMissionGroupResponse>(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(3)
        );

        DragaliaResponse<AbilityCrestBuildupPieceResponse> resp =
            await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
                "/ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrests.Aromatherapy,
                    BuildupAbilityCrestPieceList = Enumerable
                        .Range(2, 15)
                        .Select(x => new AtgenBuildupAbilityCrestPieceList()
                        {
                            BuildupPieceType = BuildupPieceTypes.Stats,
                            Step = x
                        })
                }
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.IsUpdate.Should().BeTrue();
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.CompletedMissionCount.Should()
            .BeGreaterThan(1);
        resp.Data.UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
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

        MissionReceiveMemoryEventRewardResponse response = (
            await this.Client.PostMsgpack<MissionReceiveMemoryEventRewardResponse>(
                "mission/receive_memory_event_reward",
                new MissionReceiveMemoryEventRewardRequest()
                {
                    MemoryEventMissionIdList = new[] { 10220101 }, // Participate in the Event (Toll of the Deep)
                }
            )
        ).Data;

        response
            .UpdateDataList.AbilityCrestList.Should()
            .Contain(x =>
                x.AbilityCrestId == AbilityCrests.HavingaSummerBall && x.EquipableCount == 1
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

        DragaliaResponse<MissionReceiveDailyRewardResponse> response =
            await this.Client.PostMsgpack<MissionReceiveDailyRewardResponse>(
                "mission/receive_daily_reward",
                new MissionReceiveDailyRewardRequest()
                {
                    MissionParamsList =
                    [
                        new() { DailyMissionId = missionId1, DayNo = today, },
                        new() { DailyMissionId = missionId2, DayNo = today, },
                        new() { DailyMissionId = missionId1, DayNo = yesterday, },
                    ]
                }
            );

        response
            .Data.DailyMissionList.Should()
            .BeEquivalentTo(
                [
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId1,
                        DayNo = today,
                        State = MissionState.Claimed,
                    },
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId2,
                        DayNo = today,
                        State = MissionState.Claimed,
                    },
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId2,
                        DayNo = yesterday,
                        State = MissionState.Completed,
                    },
                ],
                opts =>
                    opts.Including(x => x.DailyMissionId)
                        .Including(x => x.DayNo)
                        .Including(x => x.State)
            );
    }

    [Fact]
    public async Task ReceiveReward_Stamp_GrantsWyrmite()
    {
        // The Miracle Of Dragonyule: Clear a Boss Battle Five Times. Grants 'Splendid!' sticker
        int missionId = 10020502;

        int oldWyrmite = this
            .ApiContext.PlayerUserData.First(x => x.ViewerId == this.ViewerId)
            .Crystal;

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Id = missionId,
                Type = MissionType.MemoryEvent,
                State = MissionState.Completed,
                Progress = 5
            }
        );

        MissionReceiveMemoryEventRewardResponse response = (
            await this.Client.PostMsgpack<MissionReceiveMemoryEventRewardResponse>(
                "mission/receive_memory_event_reward",
                new MissionReceiveMemoryEventRewardRequest()
                {
                    MemoryEventMissionIdList = [missionId],
                }
            )
        ).Data;

        response.UpdateDataList.UserData.Crystal.Should().Be(oldWyrmite + 25);
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

        DragaliaResponse<MissionGetMissionListResponse> response =
            await this.Client.PostMsgpack<MissionGetMissionListResponse>(
                "mission/get_mission_list"
            );

        response
            .Data.DailyMissionList.Should()
            .BeEquivalentTo(
                [
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId,
                        DayNo = today,
                        State = MissionState.Completed
                    },
                    new DailyMissionList()
                    {
                        DailyMissionId = missionId,
                        DayNo = yesterday,
                        State = MissionState.Completed
                    }
                ],
                opts =>
                    opts.Including(x => x.DailyMissionId)
                        .Including(x => x.DayNo)
                        .Including(x => x.State)
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

        DragaliaResponse<ShopItemSummonExecResponse> response =
            await this.Client.PostMsgpack<ShopItemSummonExecResponse>(
                "shop/item_summon_exec",
                new ShopItemSummonExecRequest() { PaymentType = PaymentTypes.Wyrmite },
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
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

        MissionGetDrillMissionListResponse response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list"
            )
        ).Data;

        response.DrillMissionGroupList.Should().BeEmpty();

        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 1)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list"
            )
        ).Data;

        response.DrillMissionGroupList.Should().BeEquivalentTo([new DrillMissionGroupList(1)]);

        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 2)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list"
            )
        ).Data;

        response
            .DrillMissionGroupList.Should()
            .BeEquivalentTo([new DrillMissionGroupList(1), new DrillMissionGroupList(2)]);

        await this.AddRangeToDatabase(
            MasterAsset
                .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == 3)
                .Select(ToDbMission)
        );

        response = (
            await this.Client.PostMsgpack<MissionGetDrillMissionListResponse>(
                "mission/get_drill_mission_list"
            )
        ).Data;

        response
            .DrillMissionGroupList.Should()
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
                .Select(x => new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = x.Id,
                    State = MissionState.Claimed,
                    Type = MissionType.Drill
                })
        );

        MissionGetMissionListResponse response = (
            await this.Client.PostMsgpack<MissionGetMissionListResponse>("mission/get_mission_list")
        ).Data;

        response
            .MissionNotice.DrillMissionNotice.ReceivableRewardCount.Should()
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

        MissionGetMissionListResponse response = (
            await this.Client.PostMsgpack<MissionGetMissionListResponse>("mission/get_mission_list")
        ).Data;

        response
            .PeriodMissionList.Should()
            .HaveCount(2)
            .And.Contain(x => x.PeriodMissionId == expectedMission.Id)
            .And.Contain(x => x.PeriodMissionId == otherExpectedMission.Id);
    }
}
