using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class BuildEventTest : TestFixture
{
    public BuildEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.Client.PostMsgpack<MemoryEventActivateResponse>(
                "memory_event/activate",
                new MemoryEventActivateRequest(EventId)
            )
            .Wait();
    }

    private const int EventId = 20816;

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<BuildEventGetEventDataResponse> evtData =
            await Client.PostMsgpack<BuildEventGetEventDataResponse>(
                "build_event/get_event_data",
                new BuildEventGetEventDataRequest(EventId)
            );

        evtData.Data.BuildEventRewardList.Should().NotBeNull();
        evtData.Data.BuildEventUserData.Should().NotBeNull();
        evtData.Data.BuildEventUserData.UserBuildEventItemList.Should().HaveCount(3);
        // evtData.data.event_fort_data.Should().NotBeNull(); -- unused
        evtData.Data.EventTradeList.Should().NotBeNullOrEmpty();
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

        DragaliaResponse<BuildEventReceiveBuildPointRewardResponse> evtResp =
            await Client.PostMsgpack<BuildEventReceiveBuildPointRewardResponse>(
                "build_event/receive_build_point_reward",
                new BuildEventReceiveBuildPointRewardRequest(EventId)
            );

        evtResp
            .Data.BuildEventRewardEntityList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.Mana, 0, 3000)
            );
        evtResp.Data.BuildEventRewardList.Should().HaveCount(1);
        evtResp.Data.EntityResult.Should().NotBeNull();
        evtResp.Data.UpdateDataList.Should().NotBeNull();
    }
}
