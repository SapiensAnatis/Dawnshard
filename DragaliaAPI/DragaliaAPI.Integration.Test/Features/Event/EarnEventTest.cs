using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class EarnEventTest : TestFixture
{
    public EarnEventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    // One Starry Dragonyule
    private const int EventId = 22903;

    [Fact]
    public async Task Entry_CreatesEventData()
    {
        const int eventQuestId = 229031301; // The Angelic Herald: Standard (Solo)
        const int eventStoryId = 2290303; // The Means to Protect

        await this.AddToDatabase(
            new DbQuest() { ViewerId = this.ViewerId, QuestId = eventQuestId }
        );
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                ViewerId = this.ViewerId,
                StoryType = StoryTypes.Quest,
                StoryId = eventStoryId,
                State = StoryState.Read,
            }
        );

        this.ApiContext.PlayerEventData.Should().NotContain(x => x.ViewerId == this.ViewerId);

        await this.Client.PostMsgpack<BuildEventEntryResponse>(
            "earn_event/entry",
            new EarnEventEntryRequest(EventId)
        );

        this.ApiContext.PlayerEventData.Should()
            .ContainEquivalentOf(
                new DbPlayerEventData()
                {
                    ViewerId = this.ViewerId,
                    EventId = EventId,
                    CustomEventFlag = false,
                },
                opts => opts.Excluding(x => x.Owner)
            );

        this.ApiContext.PlayerMissions.Should()
            .HaveCount(8 + 25, because: "the event has 8 daily missions and 25 limited missions");
        this.ApiContext.PlayerMissions.ToList()
            .Should()
            .AllSatisfy(x =>
            {
                x.GroupId.Should().Be(EventId);
            });
        this.ApiContext.PlayerMissions.Should()
            .Contain(
                x => x.Id == 11650101 && x.State == MissionState.Completed,
                "this is the event participation mission"
            );
    }

    [Fact]
    public async Task GetEventData_ReturnsNullUserDataInitially()
    {
        DragaliaResponse<EarnEventGetEventDataResponse> evtData =
            await Client.PostMsgpack<EarnEventGetEventDataResponse>(
                "earn_event/get_event_data",
                new EarnEventGetEventDataRequest(EventId)
            );

        evtData
            .Data.EarnEventUserData.Should()
            .BeNull(because: "this signals the client to call /earn_event/entry");
    }

    [Fact]
    public async Task GetEventData_EntryCalled_ReturnsDataInitially()
    {
        await this.Client.PostMsgpack<BuildEventEntryResponse>(
            "earn_event/entry",
            new EarnEventEntryRequest(EventId)
        );

        DragaliaResponse<EarnEventGetEventDataResponse> evtData =
            await Client.PostMsgpack<EarnEventGetEventDataResponse>(
                "earn_event/get_event_data",
                new EarnEventGetEventDataRequest(EventId)
            );

        evtData
            .Data.EarnEventUserData.Should()
            .BeEquivalentTo(
                new EarnEventUserList()
                {
                    EventId = EventId,
                    EventPoint = 0,
                    ExchangeItem01 = 0,
                    ExchangeItem02 = 0,
                    AdventItemQuantity01 = 0
                }
            );
        evtData.Data.EventRewardList.Should().BeEmpty();
        evtData.Data.EventTradeList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        await this.Client.PostMsgpack<BuildEventEntryResponse>(
            "earn_event/entry",
            new EarnEventEntryRequest(EventId)
        );

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

        DragaliaResponse<EarnEventReceiveEventPointRewardResponse> evtResp =
            await Client.PostMsgpack<EarnEventReceiveEventPointRewardResponse>(
                "earn_event/receive_event_point_reward",
                new EarnEventReceiveEventPointRewardRequest(EventId)
            );

        evtResp
            .Data.EventRewardEntityList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.Mana, 0, 3000)
            );
        evtResp.Data.EventRewardList.Should().HaveCount(1);
        evtResp.Data.EntityResult.Should().NotBeNull();
        evtResp.Data.UpdateDataList.Should().NotBeNull();
    }
}
