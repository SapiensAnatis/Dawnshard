using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Trade;

public class TreasureTradeTest : TestFixture
{
    public TreasureTradeTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetListAll_NoTrades_ReturnsEmpty()
    {
        this.ApiContext.PlayerTrades.RemoveRange(
            this.ApiContext.PlayerTrades.Where(x => x.DeviceAccountId == DeviceAccountId)
        );

        await this.ApiContext.SaveChangesAsync();

        TreasureTradeGetListAllData response = (
            await Client.PostMsgpack<TreasureTradeGetListAllData>(
                "treasure_trade/get_list_all",
                new TreasureTradeGetListAllRequest()
            )
        ).data;

        response.user_treasure_trade_list.Should().BeEmpty();
        response.treasure_trade_all_list.Should().NotBeEmpty();
        response.treasure_trade_list.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetListAll_WithTrades_ReturnsTrades()
    {
        this.ApiContext.PlayerTrades.RemoveRange(
            this.ApiContext.PlayerTrades.Where(x => x.DeviceAccountId == DeviceAccountId)
        );

        this.ApiContext.PlayerTrades.Add(
            new DbPlayerTrade()
            {
                DeviceAccountId = DeviceAccountId,
                Id = 1000,
                Count = 1,
                Type = TradeType.Treasure,
                LastTradeTime = DateTimeOffset.UnixEpoch
            }
        );

        await this.ApiContext.SaveChangesAsync();

        TreasureTradeGetListAllData response = (
            await Client.PostMsgpack<TreasureTradeGetListAllData>(
                "treasure_trade/get_list_all",
                new TreasureTradeGetListAllRequest()
            )
        ).data;

        response.user_treasure_trade_list
            .Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new UserTreasureTradeList(1000, 1, DateTimeOffset.UnixEpoch));
        response.treasure_trade_all_list.Should().NotBeEmpty();
        response.treasure_trade_list.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Trade_ValidTrade_Trades()
    {
        IInventoryRepository repo = this.Services.GetRequiredService<IInventoryRepository>();

        DbPlayerMaterial mat = repo.AddMaterial(Materials.DamascusCrystal);
        mat.Quantity = 10;

        await this.Services.GetRequiredService<IUpdateDataService>().SaveChangesAsync();

        TreasureTradeTradeData response = (
            await Client.PostMsgpack<TreasureTradeTradeData>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(1001, 10010101, null, 1)
            )
        ).data;

        response.user_treasure_trade_list
            .Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new { treasure_trade_id = 10010101, trade_count = 1 });
        response.treasure_trade_all_list.Should().NotBeEmpty();
        response.treasure_trade_list.Should().BeNullOrEmpty();
        response.update_data_list.Should().NotBeNull();

        (await repo.GetMaterial(Materials.DamascusIngot))!.Quantity.Should().Be(1);
    }
}
