using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IDragonService
{
    Task<DragonBuyGiftToSendMultipleResponse> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request
    );
    Task<DragonSendGiftMultipleResponse> DoDragonSendGiftMultiple(
        DragonSendGiftMultipleRequest request
    );
    Task<DragonGetContactDataResponse> DoDragonGetContactData(DragonGetContactDataRequest request);
    Task<DragonBuildupResponse> DoBuildup(DragonBuildupRequest request);
    Task<DragonResetPlusCountResponse> DoDragonResetPlusCount(DragonResetPlusCountRequest request);
    Task<DragonLimitBreakResponse> DoDragonLimitBreak(DragonLimitBreakRequest request);
    Task<DragonSetLockResponse> DoDragonSetLock(DragonSetLockRequest request);
    Task<DragonSellResponse> DoDragonSell(DragonSellRequest request);
    Task<int> GetFreeGiftCount();
}
