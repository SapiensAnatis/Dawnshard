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
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = [38],
                    quest_id = 100010103,
                    repeat_setting = new()
                    {
                        repeat_count = 45,
                        repeat_type = RepeatSettingType.Specified,
                        use_item_list = [UseItem.Honey]
                    }
                }
            )
        ).data;

        startResponse.ingame_data.repeat_state.Should().Be(1);

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [] },
                    repeat_state = 1,
                }
            )
        ).data;

        recordResponse.repeat_data.repeat_count.Should().Be(1);
        recordResponse.repeat_data.repeat_key.Should().NotBeNullOrEmpty();
        recordResponse.repeat_data.repeat_state.Should().Be(1);

        DungeonStartStartAssignUnitData startResponse2 = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitData>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    request_party_setting_list = [new() { chara_id = Charas.ThePrince }],
                    quest_id = 100010103,
                    repeat_state = 1,
                    repeat_setting = null,
                }
            )
        ).data;

        startResponse2.ingame_data.repeat_state.Should().Be(1);

        DungeonRecordRecordData recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse2.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [] },
                    repeat_state = 1,
                }
            )
        ).data;

        recordResponse2.repeat_data.repeat_count.Should().Be(2);
        recordResponse2.repeat_data.repeat_key.Should().NotBeNullOrEmpty();
        recordResponse2.repeat_data.repeat_state.Should().Be(1);
    }

    [Fact]
    public async Task AutoRepeat_CallsRepeatEnd_ReturnsMergedRewardLists()
    {
        DungeonStartStartData startResponse = (
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = [38],
                    quest_id = 100010103,
                    repeat_setting = new()
                    {
                        repeat_count = 45,
                        repeat_type = RepeatSettingType.Specified,
                        use_item_list = [UseItem.Honey]
                    }
                }
            )
        ).data;

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [] },
                    repeat_state = 1,
                }
            )
        ).data;

        DungeonStartStartAssignUnitData startResponse2 = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitData>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    request_party_setting_list = [new() { chara_id = Charas.ThePrince }],
                    quest_id = 100010103,
                    repeat_state = 1,
                    repeat_setting = null,
                }
            )
        ).data;

        DungeonRecordRecordData recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse2.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [] },
                    repeat_state = 1,
                }
            )
        ).data;

        RepeatEndData repeatEndResponse = (
            await Client.PostMsgpack<RepeatEndData>("repeat/end", new RepeatEndRequest())
        ).data;

        repeatEndResponse.repeat_data.Should().BeEquivalentTo(recordResponse2.repeat_data);

        // Breaking news: lazy developer too lazy to test cumbersome merging logic
        int expectedCoin =
            recordResponse.ingame_result_data.reward_record.take_coin
            + recordResponse2.ingame_result_data.reward_record.take_coin;

        repeatEndResponse
            .ingame_result_data.reward_record.Should()
            .BeEquivalentTo(
                new RewardRecord() { take_coin = expectedCoin },
                opts => opts.Including(x => x.take_coin)
            );

        repeatEndResponse.update_data_list.Should().NotBeNull();
        repeatEndResponse
            .update_data_list.user_data.Should()
            .BeEquivalentTo(recordResponse2.update_data_list.user_data);
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

        await Client.PostMsgpack<MemoryEventActivateData>(
            "/memory_event/activate",
            new MemoryEventActivateRequest() { event_id = eventId }
        );
        DungeonStartStartData startResponse = (
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = [38],
                    quest_id = questId,
                    repeat_setting = new()
                    {
                        repeat_count = 45,
                        repeat_type = RepeatSettingType.Specified,
                        use_item_list = [UseItem.Honey]
                    }
                }
            )
        ).data;

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [], wave = 5 },
                    repeat_state = 1,
                }
            )
        ).data;

        DungeonStartStartAssignUnitData startResponse2 = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitData>(
                "/dungeon_start/start_assign_unit",
                new DungeonStartStartAssignUnitRequest()
                {
                    request_party_setting_list = [new() { chara_id = Charas.ThePrince }],
                    quest_id = questId,
                    repeat_state = 1,
                    repeat_setting = null,
                }
            )
        ).data;

        DungeonRecordRecordData recordResponse2 = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse2.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [], wave = 5 },
                    repeat_state = 1,
                }
            )
        ).data;

        RepeatEndData repeatEndResponse = (
            await Client.PostMsgpack<RepeatEndData>("repeat/end", new RepeatEndRequest())
        ).data;

        int expectedPoints =
            recordResponse.ingame_result_data.reward_record.take_accumulate_point
            + recordResponse2.ingame_result_data.reward_record.take_accumulate_point;
        int expectedBoostPoints =
            recordResponse.ingame_result_data.reward_record.take_boost_accumulate_point
            + recordResponse2.ingame_result_data.reward_record.take_boost_accumulate_point;

        repeatEndResponse
            .ingame_result_data.reward_record.take_accumulate_point.Should()
            .Be(expectedPoints);

        repeatEndResponse
            .ingame_result_data.reward_record.take_boost_accumulate_point.Should()
            .Be(expectedBoostPoints);

        repeatEndResponse
            .update_data_list.build_event_user_list.Should()
            .Contain(x => x.build_event_id == eventId);

        repeatEndResponse
            .update_data_list.build_event_user_list.First(x => x.build_event_id == eventId)
            .user_build_event_item_list.Should()
            .ContainEquivalentOf(
                new AtgenUserBuildEventItemList()
                {
                    user_build_event_item = (int)BuildEventItemType.BuildEventPoint,
                    event_item_value = expectedPoints + expectedBoostPoints
                }
            );
    }

    [Fact]
    public async Task AutoRepeat_ActiveRepeat_MypageInfoReturnsRepeatInfo()
    {
        DungeonStartStartData startResponse = (
            await Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = [38],
                    quest_id = 100010103,
                    repeat_setting = new()
                    {
                        repeat_count = 45,
                        repeat_type = RepeatSettingType.Specified,
                        use_item_list = [UseItem.Honey]
                    }
                }
            )
        ).data;

        DungeonRecordRecordData recordResponse = (
            await Client.PostMsgpack<DungeonRecordRecordData>(
                "dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = startResponse.ingame_data.dungeon_key,
                    play_record = new() { treasure_record = [] },
                    repeat_state = 1,
                }
            )
        ).data;

        MypageInfoData mypageResponse = (
            await Client.PostMsgpack<MypageInfoData>("mypage/info", new MypageInfoRequest() { })
        ).data;

        mypageResponse.repeat_data.Should().BeEquivalentTo(recordResponse.repeat_data);
    }
}
