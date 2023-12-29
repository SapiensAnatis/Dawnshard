using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Trade;

public class TreasureTradeTest : TestFixture
{
    public TreasureTradeTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetListAll_NoTrades_ReturnsEmpty()
    {
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
        await this.AddToDatabase(
            new DbPlayerTrade()
            {
                ViewerId = ViewerId,
                Id = 1000,
                Count = 1,
                Type = TradeType.Treasure,
                LastTradeTime = DateTimeOffset.UnixEpoch
            }
        );

        TreasureTradeGetListAllData response = (
            await Client.PostMsgpack<TreasureTradeGetListAllData>(
                "treasure_trade/get_list_all",
                new TreasureTradeGetListAllRequest()
            )
        ).data;

        response
            .user_treasure_trade_list.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new UserTreasureTradeList(1000, 1, DateTimeOffset.UnixEpoch));
        response.treasure_trade_all_list.Should().NotBeEmpty();
        response.treasure_trade_list.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Trade_ValidTrade_Trades()
    {
        int preTradeAmount;

        using (
            IDisposable ctx = this.Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            preTradeAmount =
                (
                    await this.Services.GetRequiredService<IInventoryRepository>()
                        .GetMaterial(Materials.DamascusIngot)
                )?.Quantity ?? 0;
        }

        TreasureTradeTradeData response = (
            await Client.PostMsgpack<TreasureTradeTradeData>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(1001, 10010101, null, 1)
            )
        ).data;

        response
            .user_treasure_trade_list.Should()
            .HaveCount(1)
            .And.Contain(x => x.treasure_trade_id == 10010101 && x.trade_count == 1);
        response.treasure_trade_all_list.Should().NotBeEmpty();
        response.treasure_trade_list.Should().BeNullOrEmpty();
        response.update_data_list.Should().NotBeNull();

        int newMatQuantity = this.ApiContext.PlayerMaterials.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId && x.MaterialId == Materials.DamascusIngot)
            .Select(x => x.Quantity)
            .First();

        newMatQuantity.Should().Be(preTradeAmount + 1);
    }

    [Fact]
    public async Task Trade_WeaponSkin_Trades()
    {
        TreasureTradeTradeData response = (
            await Client.PostMsgpack<TreasureTradeTradeData>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(1012, 10124101, null, 1)
            )
        ).data;

        response
            .user_treasure_trade_list.Should()
            .HaveCount(1)
            .And.Contain(x => x.treasure_trade_id == 10124101 && x.trade_count == 1);
        response.treasure_trade_all_list.Should().NotBeEmpty();
        response.treasure_trade_list.Should().BeNullOrEmpty();
        response.update_data_list.Should().NotBeNull();
        response
            .update_data_list.weapon_skin_list.Should()
            .Contain(x => x.weapon_skin_id == 30159921);
    }
}
