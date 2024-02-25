namespace DragaliaAPI.Integration.Test.Features.Trade;

public class EventTradeTest : TestFixture
{
    public EventTradeTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetList_FetchesEventTrades()
    {
        DragaliaResponse<EventTradeGetListData> response =
            await this.Client.PostMsgpack<EventTradeGetListData>(
                "event_trade/get_list",
                new EventTradeGetListRequest() { TradeGroupId = 10803 }
            );

        response.data.EventTradeList.Should().NotBeEmpty();
    }
}
