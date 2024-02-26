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

        TreasureTradeGetListAllResponse response = (
            await Client.PostMsgpack<TreasureTradeGetListAllResponse>("treasure_trade/get_list_all")
        ).Data;

        response.UserTreasureTradeList.Should().BeEmpty();
        response.TreasureTradeAllList.Should().NotBeEmpty();
        response.TreasureTradeList.Should().BeNullOrEmpty();
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

        TreasureTradeGetListAllResponse response = (
            await Client.PostMsgpack<TreasureTradeGetListAllResponse>("treasure_trade/get_list_all")
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new UserTreasureTradeList(1000, 1, DateTimeOffset.UnixEpoch));
        response.TreasureTradeAllList.Should().NotBeEmpty();
        response.TreasureTradeList.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Trade_ValidTrade_Trades()
    {
        int preTradeAmount;

        using (
            IDisposable ctx = this
                .Services.GetRequiredService<IPlayerIdentityService>()
                .StartUserImpersonation(viewer: ViewerId)
        )
        {
            preTradeAmount =
                (
                    await this
                        .Services.GetRequiredService<IInventoryRepository>()
                        .GetMaterial(Materials.DamascusIngot)
                )?.Quantity ?? 0;
        }

        TreasureTradeTradeResponse response = (
            await Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(1001, 10010101, null, 1)
            )
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .HaveCount(1)
            .And.Contain(x => x.TreasureTradeId == 10010101 && x.TradeCount == 1);
        response.TreasureTradeAllList.Should().NotBeEmpty();
        response.TreasureTradeList.Should().BeNullOrEmpty();
        response.UpdateDataList.Should().NotBeNull();

        int newMatQuantity = this
            .ApiContext.PlayerMaterials.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId && x.MaterialId == Materials.DamascusIngot)
            .Select(x => x.Quantity)
            .First();

        newMatQuantity.Should().Be(preTradeAmount + 1);
    }

    [Fact]
    public async Task Trade_WeaponSkin_Trades()
    {
        TreasureTradeTradeResponse response = (
            await Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(1012, 10124101, null, 1)
            )
        ).Data;

        response
            .UserTreasureTradeList.Should()
            .HaveCount(1)
            .And.Contain(x => x.TreasureTradeId == 10124101 && x.TradeCount == 1);
        response.TreasureTradeAllList.Should().NotBeEmpty();
        response.TreasureTradeList.Should().BeNullOrEmpty();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.WeaponSkinList.Should().Contain(x => x.WeaponSkinId == 30159921);
    }
}
