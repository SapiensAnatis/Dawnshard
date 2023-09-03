using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class RaidEventTest : TestFixture
{
    public RaidEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _ = Client
            .PostMsgpack<MemoryEventActivateData>(
                "memory_event/activate",
                new MemoryEventActivateRequest(EventId)
            )
            .Result;
    }

    private const int EventId = 20455;
    private const string Prefix = "raid_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<RaidEventGetEventDataData> evtData =
            await Client.PostMsgpack<RaidEventGetEventDataData>(
                $"{Prefix}/get_event_data",
                new RaidEventGetEventDataRequest(EventId)
            );

        evtData.data.raid_event_user_data.Should().NotBeNull();
        evtData.data.raid_event_reward_list.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext.PlayerEventItems.SingleAsync(
            x => x.EventId == EventId && x.Type == (int)RaidEventItemType.RaidPoint1
        );

        pointItem.Quantity += 500;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<RaidEventReceiveRaidPointRewardData> evtResp =
            await Client.PostMsgpack<RaidEventReceiveRaidPointRewardData>(
                $"{Prefix}/receive_raid_point_reward",
                new RaidEventReceiveRaidPointRewardRequest(EventId, new[] { 1001 })
            );

        evtResp.data.raid_event_reward_list.Should().HaveCount(1);
        evtResp.data.entity_result.Should().NotBeNull();
        evtResp.data.update_data_list.Should().NotBeNull();
        evtResp.data.update_data_list.raid_event_user_list.Should().HaveCount(1); // Reward is a raid event item so we test this
    }
}
