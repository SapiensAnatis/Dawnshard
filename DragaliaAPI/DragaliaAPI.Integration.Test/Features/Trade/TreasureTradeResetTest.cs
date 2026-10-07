using DragaliaAPI.Database.Entities;
using DragaliaAPI.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Trade;

public class TreasureTradeResetTest : FakeTimeProviderTestFixture
{
    // Monthly reset trade: costs 1 material, grants mana
    private const int MonthlyTradeId = 10050101;
    private const int MonthlyTradeGroupId = 1005;

    // Trade with no reset
    private const int NoResetTradeId = 10010101;
    private const int NoResetTradeGroupId = 1001;

    public TreasureTradeResetTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetListAll_MonthlyTradeBeforeLastReset_ReturnsStoredCount()
    {
        // The client works out for itself that the count has reset; the stored count is only reset on a trade.
        await this.AddToDatabase(
            new DbPlayerTrade()
            {
                ViewerId = this.ViewerId,
                Id = MonthlyTradeId,
                Type = TradeType.Treasure,
                Count = 50,
                LastTradeTime = this.FakeTimeProvider.GetLastMonthlyReset().AddSeconds(-1),
            }
        );

        TreasureTradeGetListAllResponse response = (
            await this.Client.PostMsgpack<TreasureTradeGetListAllResponse>(
                "treasure_trade/get_list_all",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .ContainSingle(x => x.TreasureTradeId == MonthlyTradeId)
            .Which.TradeCount.Should()
            .Be(50);
    }

    [Fact]
    public async Task Trade_MonthlyTradeBeforeLastReset_ResetsCount()
    {
        await this.AddToDatabase(
            new DbPlayerTrade()
            {
                ViewerId = this.ViewerId,
                Id = MonthlyTradeId,
                Type = TradeType.Treasure,
                Count = 50,
                LastTradeTime = this.FakeTimeProvider.GetLastMonthlyReset().AddSeconds(-1),
            }
        );

        TreasureTradeTradeResponse response = (
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(MonthlyTradeGroupId, MonthlyTradeId, null, 1),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .ContainSingle(x => x.TreasureTradeId == MonthlyTradeId)
            .Which.TradeCount.Should()
            .Be(1);

        (
            await this
                .ApiContext.PlayerTrades.AsNoTracking()
                .SingleAsync(
                    x => x.ViewerId == this.ViewerId && x.Id == MonthlyTradeId,
                    TestContext.Current.CancellationToken
                )
        )
            .Count.Should()
            .Be(1);
    }

    [Fact]
    public async Task Trade_MonthlyTradeAfterLastReset_AccumulatesCount()
    {
        await this.AddToDatabase(
            new DbPlayerTrade()
            {
                ViewerId = this.ViewerId,
                Id = MonthlyTradeId,
                Type = TradeType.Treasure,
                Count = 5,
                LastTradeTime = this.FakeTimeProvider.GetLastMonthlyReset().AddSeconds(1),
            }
        );

        TreasureTradeTradeResponse response = (
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(MonthlyTradeGroupId, MonthlyTradeId, null, 1),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .ContainSingle(x => x.TreasureTradeId == MonthlyTradeId)
            .Which.TradeCount.Should()
            .Be(6);
    }

    [Fact]
    public async Task Trade_MonthlyTrade_ResetsAfterMonthRollsOver()
    {
        TreasureTradeTradeResponse first = (
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(MonthlyTradeGroupId, MonthlyTradeId, null, 2),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        first
            .UserTreasureTradeList.Should()
            .ContainSingle(x => x.TreasureTradeId == MonthlyTradeId)
            .Which.TradeCount.Should()
            .Be(2);

        this.FakeTimeProvider.Advance(TimeSpan.FromDays(32));

        TreasureTradeTradeResponse second = (
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(MonthlyTradeGroupId, MonthlyTradeId, null, 1),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        second
            .UserTreasureTradeList.Should()
            .ContainSingle(x => x.TreasureTradeId == MonthlyTradeId)
            .Which.TradeCount.Should()
            .Be(1);
    }

    [Fact]
    public async Task Trade_NoResetTrade_DoesNotReset()
    {
        await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
            "treasure_trade/trade",
            new TreasureTradeTradeRequest(NoResetTradeGroupId, NoResetTradeId, null, 1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        this.FakeTimeProvider.Advance(TimeSpan.FromDays(32));

        TreasureTradeTradeResponse response = (
            await this.Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(NoResetTradeGroupId, NoResetTradeId, null, 1),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .ContainSingle(x => x.TreasureTradeId == NoResetTradeId)
            .Which.TradeCount.Should()
            .Be(2);
    }
}
