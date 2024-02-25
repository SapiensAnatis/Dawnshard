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
        DungeonStartStartData startResponse = (
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
        ).data;

        startResponse.IngameData.RepeatState.Should().Be(1);

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).data;

        recordResponse.RepeatData.RepeatCount.Should().Be(1);
        recordResponse.RepeatData.RepeatKey.Should().NotBeNullOrEmpty();
        recordResponse.RepeatData.RepeatState.Should().Be(1);

        DungeonStartStartAssignUnitData startResponse2 = (
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
        ).data;

        startResponse2.IngameData.RepeatState.Should().Be(1);

        DungeonRecordRecordData recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse2.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).data;

        recordResponse2.RepeatData.RepeatCount.Should().Be(2);
        recordResponse2.RepeatData.RepeatKey.Should().NotBeNullOrEmpty();
        recordResponse2.RepeatData.RepeatState.Should().Be(1);
    }

    [Fact]
    public async Task AutoRepeat_CallsRepeatEnd_ReturnsMergedRewardLists()
    {
        DungeonStartStartData startResponse = (
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
        ).data;

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).data;

        DungeonStartStartAssignUnitData startResponse2 = (
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
        ).data;

        DungeonRecordRecordData recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse2.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).data;

        RepeatEndData repeatEndResponse = (
            await Client.PostMsgpack<RepeatEndResponse>("repeat/end", new RepeatEndRequest())
        ).data;

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
        DungeonStartStartData startResponse = (
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
        ).data;

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [], Wave = 5 },
                    RepeatState = 1,
                }
            )
        ).data;

        DungeonStartStartAssignUnitData startResponse2 = (
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
        ).data;

        DungeonRecordRecordData recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse2.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [], Wave = 5 },
                    RepeatState = 1,
                }
            )
        ).data;

        RepeatEndData repeatEndResponse = (
            await Client.PostMsgpack<RepeatEndResponse>("repeat/end", new RepeatEndRequest())
        ).data;

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
            .UpdateDataList.BuildEventUserList.First(x => x.BuildEventId == eventId)
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
        DungeonStartStartData startResponse = (
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
        ).data;

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.IngameData.DungeonKey,
                    PlayRecord = new() { TreasureRecord = [] },
                    RepeatState = 1,
                }
            )
        ).data;

        MypageInfoData mypageResponse = (
            await Client.PostMsgpack<MypageInfoResponse>("mypage/info", new MypageInfoRequest() { })
        ).data;

        mypageResponse.RepeatData.Should().BeEquivalentTo(recordResponse.RepeatData);
    }
}
