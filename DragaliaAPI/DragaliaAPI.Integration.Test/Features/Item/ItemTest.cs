using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Item;

public class ItemTest : TestFixture
{
    public ItemTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    protected override async Task Setup()
    {
        await this.AddToDatabase(
            new DbPlayerUseItem()
            {
                ViewerId = ViewerId,
                ItemId = UseItem.Honey,
                Quantity = 50
            }
        );

        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(e => e.SetProperty(p => p.StaminaSingle, 5));
    }

    [Fact]
    public async Task GetList_ReturnsItemList()
    {
        DragaliaResponse<ItemGetListData> resp = await Client.PostMsgpack<ItemGetListData>(
            "item/get_list",
            new ItemGetListRequest()
        );

        resp.data.ItemList.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new ItemList(UseItem.Honey, 50));
    }

    [Fact]
    public async Task UseRecoveryStamina_SingleItem_RecoversStamina()
    {
        DragaliaResponse<ItemUseRecoveryStaminaData> resp =
            await Client.PostMsgpack<ItemUseRecoveryStaminaData>(
                "item/use_recovery_stamina",
                new ItemUseRecoveryStaminaRequest(
                    new List<AtgenUseItemList> { new(UseItem.Honey, 1) }
                )
            );

        resp.data.RecoverData.RecoverStaminaType.Should().Be(UseItemEffect.RecoverStamina);
        resp.data.RecoverData.RecoverStaminaPoint.Should().Be(10);
        resp.data.UpdateDataList.UserData.StaminaSingle.Should().Be(15);
        resp.data.UpdateDataList.ItemList.Should()
            .ContainEquivalentOf(new ItemList(UseItem.Honey, 49));
    }

    [Fact]
    public async Task UseRecoveryStamina_MultipleItems_RecoversStamina()
    {
        DragaliaResponse<ItemUseRecoveryStaminaData> resp =
            await Client.PostMsgpack<ItemUseRecoveryStaminaData>(
                "item/use_recovery_stamina",
                new ItemUseRecoveryStaminaRequest(
                    new List<AtgenUseItemList> { new(UseItem.Honey, 5) }
                )
            );

        resp.data.RecoverData.RecoverStaminaType.Should().Be(UseItemEffect.RecoverStamina);
        resp.data.RecoverData.RecoverStaminaPoint.Should().Be(10 * 5);
        resp.data.UpdateDataList.UserData.StaminaSingle.Should().Be(5 + (10 * 5));
        resp.data.UpdateDataList.ItemList.Should()
            .ContainEquivalentOf(new ItemList(UseItem.Honey, 45));
    }
}
