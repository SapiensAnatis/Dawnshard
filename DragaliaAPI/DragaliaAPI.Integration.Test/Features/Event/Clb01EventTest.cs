using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class Clb01EventTest : TestFixture
{
    public Clb01EventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override async Task Setup() =>
        await this.Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(EventId)
        );

    private const int EventId = 21401;
    private const string Prefix = "clb01_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<Clb01EventGetEventDataResponse> evtData =
            await Client.PostMsgpack<Clb01EventGetEventDataResponse>(
                $"{Prefix}/get_event_data",
                new Clb01EventGetEventDataRequest(EventId)
            );

        evtData.data.Clb01EventUserData.Should().NotBeNull();
        evtData.data.Clb01EventRewardList.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(x =>
                x.EventId == EventId && x.Type == (int)Clb01EventItemType.Clb01EventPoint
            );

        pointItem.Quantity += 20;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<Clb01EventReceiveClb01PointRewardResponse> evtResp =
            await Client.PostMsgpack<Clb01EventReceiveClb01PointRewardResponse>(
                $"{Prefix}/receive_clb01_point_reward",
                new Clb01EventReceiveClb01PointRewardRequest(EventId)
            );

        evtResp
            .data.Clb01EventRewardEntityList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.Material, 101001003, 5)
            );
        evtResp.data.Clb01EventRewardList.Should().HaveCount(1);
        evtResp.data.EntityResult.Should().NotBeNull();
        evtResp.data.UpdateDataList.Should().NotBeNull();
    }
}
