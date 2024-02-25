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

        DragaliaResponse<ShopGetListData> resp = await this.Client.PostMsgpack<ShopGetListData>(
            "shop/get_list",
            new ShopGetListRequest()
        );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.MaterialShopPurchase.Should().BeEmpty();
        resp.data.UserItemSummon.Should().NotBeNull();
    }

    [Fact]
    public async Task MaterialShopPurchase_ValidPurchase_AddsElement()
    {
        DragaliaResponse<ShopMaterialShopPurchaseData> resp =
            await this.Client.PostMsgpack<ShopMaterialShopPurchaseData>(
                "shop/material_shop_purchase",
                new ShopMaterialShopPurchaseRequest(
                    1000001,
                    MaterialShopType.Daily,
                    PaymentTypes.Coin,
                    1
                )
            );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.UpdateDataList.Should().NotBeNull();
        resp.data.MaterialShopPurchase.Should().HaveCount(1);
        resp.data.MaterialShopPurchase.First().GoodsId.Should().Be(1000001);
        resp.data.MaterialShopPurchase.First().BuyCount.Should().Be(1);
    }

    [Fact]
    public async Task MaterialShopPurchase_InvalidPurchase_ReturnsError()
    {
        DragaliaResponse<ShopMaterialShopPurchaseData> resp =
            await this.Client.PostMsgpack<ShopMaterialShopPurchaseData>(
                "shop/material_shop_purchase",
                new ShopMaterialShopPurchaseRequest(
                    1000001,
                    MaterialShopType.Daily,
                    (PaymentTypes)100,
                    1
                ),
                false
            );

        resp.data_headers.result_code.Should().Be(ResultCode.ShopPaymentTypeInvalid);
    }
}
