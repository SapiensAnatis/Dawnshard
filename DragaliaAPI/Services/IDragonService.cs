using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IDragonService
{
    Task<DragonBuyGiftToSendMultipleData> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request
    );
    Task<DragonSendGiftMultipleData> DoDragonSendGiftMultiple(
        DragonSendGiftMultipleRequest request
    );
    Task<DragonGetContactDataData> DoDragonGetContactData(DragonGetContactDataRequest request);
    Task<DragonBuildupData> DoBuildup(DragonBuildupRequest request);
    Task<DragonResetPlusCountData> DoDragonResetPlusCount(DragonResetPlusCountRequest request);
    Task<DragonLimitBreakData> DoDragonLimitBreak(DragonLimitBreakRequest request);
    Task<DragonSetLockData> DoDragonSetLock(DragonSetLockRequest request);
    Task<DragonSellData> DoDragonSell(DragonSellRequest request);
}
