using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class CombatEventTest : TestFixture
{
    public CombatEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override async Task Setup() =>
        await this.Client.PostMsgpack<MemoryEventActivateData>(
            "memory_event/activate",
            new MemoryEventActivateRequest(EventId)
        );

    private const int EventId = 22213;
    private const string Prefix = "combat_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<CombatEventGetEventDataData> evtData =
            await Client.PostMsgpack<CombatEventGetEventDataData>(
                $"{Prefix}/get_event_data",
                new CombatEventGetEventDataRequest(EventId)
            );

        evtData.data.combat_event_user_data.Should().NotBeNull();
        evtData.data.user_event_location_reward_list.Should().NotBeNull();
        evtData.data.event_reward_list.Should().NotBeNull();
        evtData.data.event_trade_list.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(
                x => x.EventId == EventId && x.Type == (int)CombatEventItemType.EventPoint
            );

        pointItem.Quantity += 1000;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<CombatEventReceiveEventPointRewardData> evtResp =
            await Client.PostMsgpack<CombatEventReceiveEventPointRewardData>(
                $"{Prefix}/receive_event_point_reward",
                new CombatEventReceiveEventPointRewardRequest(EventId)
            );

        evtResp.data.event_reward_entity_list.Should().HaveCount(1);
        evtResp.data.event_reward_list.Should().HaveCount(1);
        evtResp.data.entity_result.Should().NotBeNull();
        evtResp.data.update_data_list.Should().NotBeNull();
        evtResp.data.update_data_list.combat_event_user_list.Should().HaveCount(1); // Reward is event item so we check this
    }

    [Fact]
    public async Task ReceiveEventLocationRewards_ReturnsEventLocationRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(
                x => x.EventId == EventId && x.Type == (int)Clb01EventItemType.Clb01EventPoint
            );

        pointItem.Quantity += 500;

        ApiContext.PlayerQuests.RemoveRange(
            ApiContext.PlayerQuests.Where(x => x.ViewerId == ViewerId)
        );

        ApiContext.PlayerQuests.Add(
            new DbQuest
            {
                ViewerId = ViewerId,
                QuestId = 222130103,
                State = 3
            }
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<CombatEventReceiveEventLocationRewardData> evtResp =
            await Client.PostMsgpack<CombatEventReceiveEventLocationRewardData>(
                $"{Prefix}/receive_event_location_reward",
                new CombatEventReceiveEventLocationRewardRequest(EventId, 2221302)
            );

        evtResp.data.event_location_reward_entity_list.Should().HaveCount(9);
        evtResp.data.user_event_location_reward_list.Should().HaveCount(1);
        evtResp.data.update_data_list.Should().NotBeNull();
    }
}
