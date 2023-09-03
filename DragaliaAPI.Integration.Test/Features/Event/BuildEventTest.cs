using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class BuildEventTest : TestFixture
{
    public BuildEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _ = Client
            .PostMsgpack<MemoryEventActivateData>(
                "memory_event/activate",
                new MemoryEventActivateRequest(EventId)
            )
            .Result;
    }

    private const int EventId = 20816;

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<BuildEventGetEventDataData> evtData =
            await Client.PostMsgpack<BuildEventGetEventDataData>(
                "build_event/get_event_data",
                new BuildEventGetEventDataRequest(EventId)
            );

        evtData.data.build_event_reward_list.Should().NotBeNull();
        evtData.data.build_event_user_data.Should().NotBeNull();
        evtData.data.build_event_user_data.user_build_event_item_list.Should().HaveCount(3);
        // evtData.data.event_fort_data.Should().NotBeNull(); -- unused
        evtData.data.event_trade_list.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext.PlayerEventItems.SingleAsync(
            x => x.EventId == EventId && x.Type == (int)BuildEventItemType.BuildEventPoint
        );

        pointItem.Quantity += 10;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<BuildEventReceiveBuildPointRewardData> evtResp =
            await Client.PostMsgpack<BuildEventReceiveBuildPointRewardData>(
                "build_event/receive_build_point_reward",
                new BuildEventReceiveBuildPointRewardRequest(EventId)
            );

        evtResp.data.build_event_reward_entity_list
            .Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.Mana, 0, 3000)
            );
        evtResp.data.build_event_reward_list.Should().HaveCount(1);
        evtResp.data.entity_result.Should().NotBeNull();
        evtResp.data.update_data_list.Should().NotBeNull();
    }
}
