using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class BuildEventTest : TestFixture
{
    public BuildEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override async Task Setup() =>
        await this.Client.PostMsgpack<MemoryEventActivateData>(
            "memory_event/activate",
            new MemoryEventActivateRequest(EventId)
        );

    private const int EventId = 20816;

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<BuildEventGetEventDataData> evtData =
            await Client.PostMsgpack<BuildEventGetEventDataData>(
                "build_event/get_event_data",
                new BuildEventGetEventDataRequest(EventId)
            );

        evtData.data.BuildEventRewardList.Should().NotBeNull();
        evtData.data.BuildEventUserData.Should().NotBeNull();
        evtData.data.BuildEventUserData.UserBuildEventItemList.Should().HaveCount(3);
        // evtData.data.event_fort_data.Should().NotBeNull(); -- unused
        evtData.data.EventTradeList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(x =>
                x.EventId == EventId && x.Type == (int)BuildEventItemType.BuildEventPoint
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

        evtResp
            .data.BuildEventRewardEntityList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.Mana, 0, 3000)
            );
        evtResp.data.BuildEventRewardList.Should().HaveCount(1);
        evtResp.data.EntityResult.Should().NotBeNull();
        evtResp.data.UpdateDataList.Should().NotBeNull();
    }
}
