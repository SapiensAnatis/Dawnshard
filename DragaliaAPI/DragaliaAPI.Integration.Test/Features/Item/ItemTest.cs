using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Item;

public class ItemTest : TestFixture
{
    public ItemTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.AddToDatabase(
                new DbPlayerUseItem()
                {
                    ViewerId = ViewerId,
                    ItemId = UseItem.Honey,
                    Quantity = 50
                }
            )
            .Wait();

        this.ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdate(e => e.SetProperty(p => p.StaminaSingle, 5));
    }

    [Fact]
    public async Task GetList_ReturnsItemList()
    {
        DragaliaResponse<ItemGetListResponse> resp = await Client.PostMsgpack<ItemGetListResponse>(
            "item/get_list"
        );

        resp.Data.ItemList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new ItemList(UseItem.Honey, 50));
    }

    [Fact]
    public async Task UseRecoveryStamina_SingleItem_RecoversStamina()
    {
        DragaliaResponse<ItemUseRecoveryStaminaResponse> resp =
            await Client.PostMsgpack<ItemUseRecoveryStaminaResponse>(
                "item/use_recovery_stamina",
                new ItemUseRecoveryStaminaRequest(
                    new List<AtgenUseItemList> { new(UseItem.Honey, 1) }
                )
            );

        resp.Data.RecoverData.RecoverStaminaType.Should().Be(UseItemEffect.RecoverStamina);
        resp.Data.RecoverData.RecoverStaminaPoint.Should().Be(10);
        resp.Data.UpdateDataList.UserData.StaminaSingle.Should().Be(15);
        resp.Data.UpdateDataList.ItemList.Should()
            .ContainEquivalentOf(new ItemList(UseItem.Honey, 49));
    }

    [Fact]
    public async Task UseRecoveryStamina_MultipleItems_RecoversStamina()
    {
        DragaliaResponse<ItemUseRecoveryStaminaResponse> resp =
            await Client.PostMsgpack<ItemUseRecoveryStaminaResponse>(
                "item/use_recovery_stamina",
                new ItemUseRecoveryStaminaRequest(
                    new List<AtgenUseItemList> { new(UseItem.Honey, 5) }
                )
            );

        resp.Data.RecoverData.RecoverStaminaType.Should().Be(UseItemEffect.RecoverStamina);
        resp.Data.RecoverData.RecoverStaminaPoint.Should().Be(10 * 5);
        resp.Data.UpdateDataList.UserData.StaminaSingle.Should().Be(5 + (10 * 5));
        resp.Data.UpdateDataList.ItemList.Should()
            .ContainEquivalentOf(new ItemList(UseItem.Honey, 45));
    }
}
