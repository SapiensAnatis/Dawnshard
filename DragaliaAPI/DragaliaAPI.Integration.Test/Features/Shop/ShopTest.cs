using DragaliaAPI.Features.Shop;

namespace DragaliaAPI.Integration.Test.Features.Shop;

public class ShopTest : TestFixture
{
    public ShopTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetList_NoPurchases_IsEmpty()
    {
        this.ApiContext.RemoveRange(
            this.ApiContext.PlayerPurchases.Where(x => x.ViewerId == ViewerId)
        );

        await this.ApiContext.SaveChangesAsync();

        DragaliaResponse<ShopGetListResponse> resp =
            await this.Client.PostMsgpack<ShopGetListResponse>("shop/get_list");

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.MaterialShopPurchase.Should().BeEmpty();
        resp.Data.UserItemSummon.Should().NotBeNull();
    }

    [Fact]
    public async Task MaterialShopPurchase_ValidPurchase_AddsElement()
    {
        DragaliaResponse<ShopMaterialShopPurchaseResponse> resp =
            await this.Client.PostMsgpack<ShopMaterialShopPurchaseResponse>(
                "shop/material_shop_purchase",
                new ShopMaterialShopPurchaseRequest(
                    1000001,
                    MaterialShopType.Daily,
                    PaymentTypes.Coin,
                    1
                )
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
        resp.Data.UpdateDataList.Should().NotBeNull();
        resp.Data.MaterialShopPurchase.Should().HaveCount(1);
        resp.Data.MaterialShopPurchase.First().GoodsId.Should().Be(1000001);
        resp.Data.MaterialShopPurchase.First().BuyCount.Should().Be(1);
    }

    [Fact]
    public async Task MaterialShopPurchase_InvalidPurchase_ReturnsError()
    {
        DragaliaResponse<ShopMaterialShopPurchaseResponse> resp =
            await this.Client.PostMsgpack<ShopMaterialShopPurchaseResponse>(
                "shop/material_shop_purchase",
                new ShopMaterialShopPurchaseRequest(
                    1000001,
                    MaterialShopType.Daily,
                    (PaymentTypes)100,
                    1
                ),
                false
            );

        resp.DataHeaders.ResultCode.Should().Be(ResultCode.ShopPaymentTypeInvalid);
    }
}
