using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Trade;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Item;

public class ItemService(
    IItemRepository itemRepository,
    ILogger<ItemService> logger,
    IUserService userService,
    IPaymentService paymentService
) : IItemService
{
    public async Task<IEnumerable<ItemList>> GetItemList()
    {
        return (await itemRepository.Items.ToListAsync()).Select(
            x => new ItemList(x.ItemId, x.Quantity)
        );
    }

    public async Task<AtgenRecoverData> UseItems(IEnumerable<AtgenUseItemList> items)
    {
        logger.LogDebug("Processing items {@useItems}", items);

        UseItemEffect effect = UseItemEffect.None;
        int totalQuantity = 0;

        foreach (AtgenUseItemList item in items)
        {
            UseItemData data = MasterAsset.UseItem[item.item_id];
            if (effect == UseItemEffect.None)
                effect = data.ItemEffect;
            totalQuantity += data.ItemEffectValue * item.item_quantity;

            await paymentService.ProcessPayment(
                new Entity(EntityTypes.Item, (int)item.item_id, item.item_quantity)
            );
        }

        switch (effect)
        {
            case UseItemEffect.None:
                throw new DragaliaException(ResultCode.CommonInvalidArgument, "None UseItemEffect");
            case UseItemEffect.RecoverStamina:
                await userService.AddStamina(StaminaType.Single, totalQuantity);
                break;
            case UseItemEffect.RecoverMulti:
                await userService.AddStamina(StaminaType.Multi, totalQuantity);
                break;
            case UseItemEffect.QuestSkip:
                await userService.AddQuestSkipPoint(totalQuantity);
                break;
            default:
                throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid UseItemEffect"
                );
        }

        return new AtgenRecoverData(effect, totalQuantity);
    }
}
