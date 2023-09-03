using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Item;

public class ItemTest : TestFixture
{
    public ItemTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        ApiContext.PlayerUseItems.Where(x => x.DeviceAccountId == DeviceAccountId).ExecuteDelete();

        ApiContext.PlayerUseItems.Add(
            new DbPlayerUseItem()
            {
                DeviceAccountId = DeviceAccountId,
                ItemId = UseItem.Honey,
                Quantity = 50
            }
        );

        DbPlayerUserData userData = ApiContext.PlayerUserData.Single(
            x => x.DeviceAccountId == DeviceAccountId
        );

        userData.StaminaSingle = 5;

        ApiContext.SaveChanges();
        ApiContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetList_ReturnsItemList()
    {
        DragaliaResponse<ItemGetListData> resp = await Client.PostMsgpack<ItemGetListData>(
            "item/get_list",
            new ItemGetListRequest()
        );

        resp.data.item_list
            .Should()
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

        resp.data.recover_data.recover_stamina_type.Should().Be(UseItemEffect.RecoverStamina);
        resp.data.recover_data.recover_stamina_point.Should().Be(10);
        resp.data.update_data_list.user_data.stamina_single.Should().Be(15);
        resp.data.update_data_list.item_list
            .Should()
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

        resp.data.recover_data.recover_stamina_type.Should().Be(UseItemEffect.RecoverStamina);
        resp.data.recover_data.recover_stamina_point.Should().Be(10 * 5);
        resp.data.update_data_list.user_data.stamina_single.Should().Be(5 + (10 * 5));
        resp.data.update_data_list.item_list
            .Should()
            .ContainEquivalentOf(new ItemList(UseItem.Honey, 45));
    }
}
