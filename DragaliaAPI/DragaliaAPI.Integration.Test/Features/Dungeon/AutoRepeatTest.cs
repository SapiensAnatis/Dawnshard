using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.Dungeon;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class AutoRepeatTest : TestFixture
{
    public AutoRepeatTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task AutoRepeat_RecordReturnsRepeatKey()
    {
        DungeonStartStartResponse startResponse = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = [38],
                    QuestId = 100010103,
                    RepeatSetting = new()
                    {
                        RepeatCount = 45,
                        RepeatType = RepeatSettingType.Specified,
                        UseItemList = [UseItem.Honey]
                    }
                }
            )
        ).Data;

        startResponse.IngameData.RepeatState.Should().Be(1);

        DungeonRecordRecordResponse recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).Data;

        recordResponse.RepeatData.RepeatCount.Should().Be(1);
        recordResponse.RepeatData.RepeatKey.Should().NotBeNullOrEmpty();
        recordResponse.RepeatData.RepeatState.Should().Be(1);

        DungeonStartStartAssignUnitResponse startResponse2 = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    RequestPartySettingList = [new() { CharaId = Charas.ThePrince }],
                    QuestId = 100010103,
                    RepeatState = 1,
                    RepeatSetting = null,
                }
            )
        ).Data;

        startResponse2.IngameData.RepeatState.Should().Be(1);

        DungeonRecordRecordResponse recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse2.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).Data;

        recordResponse2.RepeatData.RepeatCount.Should().Be(2);
        recordResponse2.RepeatData.RepeatKey.Should().NotBeNullOrEmpty();
        recordResponse2.RepeatData.RepeatState.Should().Be(1);
    }

    [Fact]
    public async Task AutoRepeat_StaleRepeatKey_RecordReturnsRepeatKey()
    {
        DungeonStartStartResponse startResponse = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = [38],
                    QuestId = 100010103,
                    RepeatSetting = new()
                    {
                        RepeatCount = 45,
                        RepeatType = RepeatSettingType.Specified,
                        UseItemList = [UseItem.Honey]
                    }
                }
            )
        ).Data;

        startResponse.IngameData.RepeatState.Should().Be(1);
        string staleRepeatKey = Guid.NewGuid().ToString();

        DungeonRecordRecordResponse recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    RepeatKey = staleRepeatKey,
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).Data;

        recordResponse.RepeatData.Should().NotBeNull();
        recordResponse.RepeatData.RepeatCount.Should().Be(1);
        recordResponse
            .RepeatData.RepeatKey.Should()
            .NotBeNullOrEmpty()
            .And.NotBeEquivalentTo(staleRepeatKey);
        recordResponse.RepeatData.RepeatState.Should().Be(1);
    }

    [Fact]
    public async Task AutoRepeat_CallsRepeatEnd_ReturnsMergedRewardLists()
    {
        DungeonStartStartResponse startResponse = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = [38],
                    QuestId = 100010103,
                    RepeatSetting = new()
                    {
                        RepeatCount = 45,
                        RepeatType = RepeatSettingType.Specified,
                        UseItemList = [UseItem.Honey]
                    }
                }
            )
        ).Data;

        DungeonRecordRecordResponse recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).Data;

        DungeonStartStartAssignUnitResponse startResponse2 = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    RequestPartySettingList = [new() { CharaId = Charas.ThePrince }],
                    QuestId = 100010103,
                    RepeatState = 1,
                    RepeatSetting = null,
                }
            )
        ).Data;

        DungeonRecordRecordResponse recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse2.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).Data;

        RepeatEndResponse repeatEndResponse = (
            await Client.PostMsgpack<RepeatEndResponse>("repeat/end")
        ).Data;

        repeatEndResponse.RepeatData.Should().BeEquivalentTo(recordResponse2.RepeatData);

        // Breaking news: lazy developer too lazy to test cumbersome merging logic
        int expectedCoin =
            recordResponse.IngameResultData.RewardRecord.TakeCoin
            + recordResponse2.IngameResultData.RewardRecord.TakeCoin;

        repeatEndResponse
            .IngameResultData.RewardRecord.Should()
            .BeEquivalentTo(
                new RewardRecord() { TakeCoin = expectedCoin },
                opts => opts.Including(x => x.TakeCoin)
            );

        repeatEndResponse.UpdateDataList.Should().NotBeNull();
        repeatEndResponse
            .UpdateDataList.UserData.Should()
            .BeEquivalentTo(recordResponse2.UpdateDataList.UserData);
    }

    [Fact]
    public async Task AutoRepeat_EventMission_GrantsAccumulatedPoints()
    {
        int eventId = 20816;
        int questId = 208160502; // Flames of Reflection -- The Path To Mastery: Master

        await AddToDatabase(
            new DbQuest()
            {
                QuestId = questId,
                State = 0,
                ViewerId = ViewerId
            }
        );

        await Client.PostMsgpack<MemoryEventActivateResponse>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { EventId = eventId }
        );
        DungeonStartStartResponse startResponse = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = [38],
                    QuestId = questId,
                    RepeatSetting = new()
                    {
                        RepeatCount = 45,
                        RepeatType = RepeatSettingType.Specified,
                        UseItemList = [UseItem.Honey]
                    }
                }
            )
        ).Data;

        DungeonRecordRecordResponse recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [], Wave = 5 },
                    RepeatState = 1,
                }
            )
        ).Data;

        DungeonStartStartAssignUnitResponse startResponse2 = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    RequestPartySettingList = [new() { CharaId = Charas.ThePrince }],
                    QuestId = questId,
                    RepeatState = 1,
                    RepeatSetting = null,
                }
            )
        ).Data;

        DungeonRecordRecordResponse recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse2.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [], Wave = 5 },
                    RepeatState = 1,
                }
            )
        ).Data;

        RepeatEndResponse repeatEndResponse = (
            await Client.PostMsgpack<RepeatEndResponse>("repeat/end")
        ).Data;

        int expectedPoints =
            recordResponse.IngameResultData.RewardRecord.TakeAccumulatePoint
            + recordResponse2.IngameResultData.RewardRecord.TakeAccumulatePoint;
        int expectedBoostPoints =
            recordResponse.IngameResultData.RewardRecord.TakeBoostAccumulatePoint
            + recordResponse2.IngameResultData.RewardRecord.TakeBoostAccumulatePoint;

        repeatEndResponse
            .IngameResultData.RewardRecord.TakeAccumulatePoint.Should()
            .Be(expectedPoints);

        repeatEndResponse
            .IngameResultData.RewardRecord.TakeBoostAccumulatePoint.Should()
            .Be(expectedBoostPoints);

        repeatEndResponse
            .UpdateDataList.BuildEventUserList.Should()
            .Contain(x => x.BuildEventId == eventId);

        repeatEndResponse
            .UpdateDataList.BuildEventUserList!.First(x => x.BuildEventId == eventId)
            .UserBuildEventItemList.Should()
            .ContainEquivalentOf(
                new AtgenUserBuildEventItemList()
                {
                    UserBuildEventItem = (int)BuildEventItemType.BuildEventPoint,
                    EventItemValue = expectedPoints + expectedBoostPoints
                }
            );
    }

    [Fact]
    public async Task AutoRepeat_ActiveRepeat_MypageInfoReturnsRepeatInfo()
    {
        DungeonStartStartResponse startResponse = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = [38],
                    QuestId = 100010103,
                    RepeatSetting = new()
                    {
                        RepeatCount = 45,
                        RepeatType = RepeatSettingType.Specified,
                        UseItemList = [UseItem.Honey]
                    }
                }
            )
        ).Data;

        DungeonRecordRecordResponse recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).Data;

        MypageInfoResponse mypageResponse = (
            await Client.PostMsgpack<MypageInfoResponse>("mypage/info")
        ).Data;

        mypageResponse.RepeatData.Should().BeEquivalentTo(recordResponse.RepeatData);
    }
}
