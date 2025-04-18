﻿using DragaliaAPI.Database.Entities;
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
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        TreasureTradeGetListAllResponse response = (
            await Client.PostMsgpack<TreasureTradeGetListAllResponse>(
                "treasure_trade/get_list_all",
                cancellationToken: TestContext.Current.CancellationToken
            )
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
                LastTradeTime = DateTimeOffset.UnixEpoch,
            }
        );

        TreasureTradeGetListAllResponse response = (
            await Client.PostMsgpack<TreasureTradeGetListAllResponse>(
                "treasure_trade/get_list_all",
                cancellationToken: TestContext.Current.CancellationToken
            )
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
                new TreasureTradeTradeRequest(1001, 10010101, null, 1),
                cancellationToken: TestContext.Current.CancellationToken
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
                new TreasureTradeTradeRequest(1012, 10124101, null, 1),
                cancellationToken: TestContext.Current.CancellationToken
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

    [Fact]
    public async Task Trade_MultiDragon_AlmostFull_Trades()
    {
        this.ApiContext.PlayerDragonData.ExecuteDelete();
        this.ApiContext.PlayerUserData.ExecuteUpdate(e =>
            e.SetProperty(x => x.MaxDragonQuantity, 2)
        );

        int highBrunhildaTrade = 10030302;

        TreasureTradeTradeResponse response = (
            await Client.PostMsgpack<TreasureTradeTradeResponse>(
                "treasure_trade/trade",
                new TreasureTradeTradeRequest(1003, highBrunhildaTrade, null, 4),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.DragonList.Should().HaveCount(2);
        response.EntityResult.OverPresentEntityList.Should().HaveCount(2);

        this.ApiContext.PlayerDragonData.ToList()
            .Should()
            .HaveCount(2)
            .And.AllSatisfy(x => x.DragonId.Should().Be(DragonId.HighBrunhilda));
        this.ApiContext.PlayerPresents.ToList()
            .Should()
            .HaveCount(2)
            .And.AllSatisfy(x =>
            {
                x.EntityType.Should().Be(EntityTypes.Dragon);
                x.EntityId.Should().Be((int)DragonId.HighBrunhilda);
            });
    }
}
