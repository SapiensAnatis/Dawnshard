using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

#pragma warning disable CA1861 // Prefer 'static readonly' fields over constant array arguments if the called method is called repeatedly and is not mutating the passed array

public class RaidEventTest : TestFixture
{
    public RaidEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        this.Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(EventId)
        )
            .Wait();
    }

    private const int EventId = 20455;
    private const string Prefix = "raid_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<RaidEventGetEventDataResponse> evtData =
            await Client.PostMsgpack<RaidEventGetEventDataResponse>(
                $"{Prefix}/get_event_data",
                new RaidEventGetEventDataRequest(EventId)
            );

        evtData.Data.RaidEventUserData.Should().NotBeNull();
        evtData.Data.RaidEventRewardList.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(x => x.EventId == EventId && x.Type == (int)RaidEventItemType.RaidPoint1);

        pointItem.Quantity += 500;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<RaidEventReceiveRaidPointRewardResponse> evtResp =
            await Client.PostMsgpack<RaidEventReceiveRaidPointRewardResponse>(
                $"{Prefix}/receive_raid_point_reward",
                new RaidEventReceiveRaidPointRewardRequest(EventId, new[] { 1001 })
            );

        evtResp.Data.RaidEventRewardList.Should().HaveCount(1);
        evtResp.Data.EntityResult.Should().NotBeNull();
        evtResp.Data.UpdateDataList.Should().NotBeNull();
        evtResp.Data.UpdateDataList.RaidEventUserList.Should().HaveCount(1); // Reward is a raid event item so we test this
    }

    [Fact]
    public async Task Entry_EventHasNoItems_InitializesUserData()
    {
        const int fracturedFuturesId = 20427;

        await this.Client.PostMsgpack(
            "memory_event/activate",
            new MemoryEventActivateRequest(fracturedFuturesId)
        );

        DragaliaResponse<RaidEventGetEventDataResponse> response =
            await this.Client.PostMsgpack<RaidEventGetEventDataResponse>(
                "raid_event/get_event_data",
                new RaidEventGetEventDataRequest(fracturedFuturesId)
            );

        response.Data.RaidEventUserData.Should().NotBeNull();
    }
}
