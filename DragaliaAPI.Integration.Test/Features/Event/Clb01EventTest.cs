using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Event;

public class Clb01EventTest : TestFixture
{
    public Clb01EventTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _ = Client
            .PostMsgpack<MemoryEventActivateData>(
                "memory_event/activate",
                new MemoryEventActivateRequest(EventId)
            )
            .Result;
    }

    private const int EventId = 21401;
    private const string Prefix = "clb01_event";

    [Fact]
    public async Task GetEventData_ReturnsEventData()
    {
        DragaliaResponse<Clb01EventGetEventDataData> evtData =
            await Client.PostMsgpack<Clb01EventGetEventDataData>(
                $"{Prefix}/get_event_data",
                new Clb01EventGetEventDataRequest(EventId)
            );

        evtData.data.clb_01_event_user_data.Should().NotBeNull();
        evtData.data.clb_01_event_reward_list.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveEventRewards_ReturnsEventRewards()
    {
        DbPlayerEventItem pointItem = await ApiContext.PlayerEventItems.SingleAsync(
            x => x.EventId == EventId && x.Type == (int)Clb01EventItemType.Clb01EventPoint
        );

        pointItem.Quantity += 20;

        ApiContext.PlayerEventRewards.RemoveRange(
            ApiContext.PlayerEventRewards.Where(x => x.EventId == EventId)
        );

        await ApiContext.SaveChangesAsync();

        DragaliaResponse<Clb01EventReceiveClb01PointRewardData> evtResp =
            await Client.PostMsgpack<Clb01EventReceiveClb01PointRewardData>(
                $"{Prefix}/receive_clb01_point_reward",
                new Clb01EventReceiveClb01PointRewardRequest(EventId)
            );

        evtResp.data.clb_01_event_reward_entity_list
            .Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.Material, 101001003, 5)
            );
        evtResp.data.clb_01_event_reward_list.Should().HaveCount(1);
        evtResp.data.entity_result.Should().NotBeNull();
        evtResp.data.update_data_list.Should().NotBeNull();
    }
}
