using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class CombatEventTest : TestFixture
{
    public CombatEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);

        this.Client.PostMsgpack<MemoryEventActivateResponse>(
                "memory_event/activate",
                new MemoryEventActivateRequest(EventId)
            )
            .Wait();
    }

    private const int EventId = 22213;
    private const string Prefix = "combat_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<CombatEventGetEventDataResponse> evtData =
            await Client.PostMsgpack<CombatEventGetEventDataResponse>(
                $"{Prefix}/get_event_data",
                new CombatEventGetEventDataRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        evtData.Data.CombatEventUserData.Should().NotBeNull();
        evtData.Data.UserEventLocationRewardList.Should().NotBeNull();
        evtData.Data.EventRewardList.Should().NotBeNull();
        evtData.Data.EventTradeList.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.Where(x => x.ViewerId == this.ViewerId)
            .AsTracking()
            .SingleAsync(
                x => x.EventId == EventId && x.Type == (int)CombatEventItemType.EventPoint,
                cancellationToken: TestContext.Current.CancellationToken
            );

        pointItem.Quantity += 1000;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<CombatEventReceiveEventPointRewardResponse> evtResp =
            await Client.PostMsgpack<CombatEventReceiveEventPointRewardResponse>(
                $"{Prefix}/receive_event_point_reward",
                new CombatEventReceiveEventPointRewardRequest(EventId),
                cancellationToken: TestContext.Current.CancellationToken
            );

        evtResp.Data.EventRewardEntityList.Should().HaveCount(1);
        evtResp.Data.EventRewardList.Should().HaveCount(1);
        evtResp.Data.EntityResult.Should().NotBeNull();
        evtResp.Data.UpdateDataList.Should().NotBeNull();
        evtResp.Data.UpdateDataList.CombatEventUserList.Should().HaveCount(1); // Reward is event item so we check this
    }

    [Fact]
    public async Task ReceiveEventLocationRewards_ReturnsEventLocationRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext
            .PlayerEventItems.AsTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .SingleAsync(
                x => x.EventId == EventId && x.Type == (int)Clb01EventItemType.Clb01EventPoint,
                cancellationToken: TestContext.Current.CancellationToken
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
                State = 3,
            }
        );

        await ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<CombatEventReceiveEventLocationRewardResponse> evtResp =
            await Client.PostMsgpack<CombatEventReceiveEventLocationRewardResponse>(
                $"{Prefix}/receive_event_location_reward",
                new CombatEventReceiveEventLocationRewardRequest(EventId, 2221302),
                cancellationToken: TestContext.Current.CancellationToken
            );

        evtResp.Data.EventLocationRewardEntityList.Should().HaveCount(9);
        evtResp.Data.UserEventLocationRewardList.Should().HaveCount(1);
        evtResp.Data.UpdateDataList.Should().NotBeNull();
    }
}
