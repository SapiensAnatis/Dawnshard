using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
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
    IUserDataRepository userDataRepository,
    ILogger<ItemService> logger
) : IItemService
{
    private const int MaxSingleStamina = 999;
    private const int MaxMultiStamina = 99;
    private const int MaxQuestSkipPoint = 400;

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
            totalQuantity += data.ItemEffectValue;
        }

        DateTimeOffset time = DateTimeOffset.UtcNow;
        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();
        switch (effect)
        {
            case UseItemEffect.None:
                throw new DragaliaException(ResultCode.CommonInvalidArgument, "None UseItemEffect");
            case UseItemEffect.RecoverStamina:
                userData.StaminaSingle = Math.Min(
                    MaxSingleStamina,
                    userData.StaminaSingle + totalQuantity
                );
                userData.LastStaminaSingleUpdateTime = time;
                break;
            case UseItemEffect.RecoverMulti:
                userData.StaminaMulti = Math.Min(
                    MaxMultiStamina,
                    userData.StaminaMulti + totalQuantity
                );
                userData.LastStaminaMultiUpdateTime = time;
                break;
            case UseItemEffect.QuestSkip:
                userData.QuestSkipPoint = Math.Min(
                    MaxQuestSkipPoint,
                    userData.QuestSkipPoint + totalQuantity
                );
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
