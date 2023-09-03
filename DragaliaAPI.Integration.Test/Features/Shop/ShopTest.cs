using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Integration.Test.Features.Shop;

public class ShopTest : TestFixture
{
    public ShopTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetList_NoPurchases_IsEmpty()
    {
        this.ApiContext.RemoveRange(
            this.ApiContext.PlayerPurchases.Where(x => x.DeviceAccountId == DeviceAccountId)
        );

        await this.ApiContext.SaveChangesAsync();

        DragaliaResponse<ShopGetListData> resp = await this.Client.PostMsgpack<ShopGetListData>(
            "shop/get_list",
            new ShopGetListRequest()
        );

        resp.data_headers.result_code.Should().Be(ResultCode.Success);
        resp.data.material_shop_purchase.Should().BeEmpty();
        resp.data.user_item_summon.Should().NotBeNull();
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
        resp.data.update_data_list.Should().NotBeNull();
        resp.data.material_shop_purchase.Should().HaveCount(1);
        resp.data.material_shop_purchase.First().goods_id.Should().Be(1000001);
        resp.data.material_shop_purchase.First().buy_count.Should().Be(1);
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
