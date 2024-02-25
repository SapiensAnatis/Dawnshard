using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class CombatEventTest : TestFixture
{
    public CombatEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override async Task Setup() =>
        await this.Client.PostMsgpack<MemoryEventActivateResponse>(
            "memory_event/activate",
            new MemoryEventActivateRequest(EventId)
        );

    private const int EventId = 22213;
    private const string Prefix = "combat_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<CombatEventGetEventDataData> evtData =
            await Client.PostMsgpack<CombatEventGetEventDataResponse>(
                $"{Prefix}/get_event_data",
                new CombatEventGetEventDataRequest(EventId)
            );

        evtData.data.CombatEventUserData.Should().NotBeNull();
        evtData.data.UserEventLocationRewardList.Should().NotBeNull();
        evtData.data.EventRewardList.Should().NotBeNull();
        evtData.data.EventTradeList.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(x =>
                x.EventId == EventId && x.Type == (int)CombatEventItemType.EventPoint
            );

        pointItem.Quantity += 1000;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<CombatEventReceiveEventPointRewardData> evtResp =
            await Client.PostMsgpack<CombatEventReceiveEventPointRewardResponse>(
                $"{Prefix}/receive_event_point_reward",
                new CombatEventReceiveEventPointRewardRequest(EventId)
            );

        evtResp.data.EventRewardEntityList.Should().HaveCount(1);
        evtResp.data.EventRewardList.Should().HaveCount(1);
        evtResp.data.EntityResult.Should().NotBeNull();
        evtResp.data.UpdateDataList.Should().NotBeNull();
        evtResp.data.UpdateDataList.CombatEventUserList.Should().HaveCount(1); // Reward is event item so we check this
    }

    [Fact]
    public async Task ReceiveEventLocationRewards_ReturnsEventLocationRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .SingleAsync(x =>
                x.EventId == EventId && x.Type == (int)Clb01EventItemType.Clb01EventPoint
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
            await Client.PostMsgpack<CombatEventReceiveEventLocationRewardResponse>(
                $"{Prefix}/receive_event_location_reward",
                new CombatEventReceiveEventLocationRewardRequest(EventId, 2221302)
            );

        evtResp.data.EventLocationRewardEntityList.Should().HaveCount(9);
        evtResp.data.UserEventLocationRewardList.Should().HaveCount(1);
        evtResp.data.UpdateDataList.Should().NotBeNull();
    }
}
