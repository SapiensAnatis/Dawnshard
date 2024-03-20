using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IDragonService
{
    Task<DragonBuyGiftToSendMultipleResponse> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request,
        CancellationToken cancellationToken
    );
    Task<DragonSendGiftMultipleResponse> DoDragonSendGiftMultiple(
        DragonSendGiftMultipleRequest request,
        CancellationToken cancellationToken
    );
    Task<DragonGetContactDataResponse> DoDragonGetContactData();
    Task<DragonBuildupResponse> DoBuildup(
        DragonBuildupRequest request,
        CancellationToken cancellationToken
    );
    Task<DragonResetPlusCountResponse> DoDragonResetPlusCount(
        DragonResetPlusCountRequest request,
        CancellationToken cancellationToken
    );
    Task<DragonLimitBreakResponse> DoDragonLimitBreak(
        DragonLimitBreakRequest request,
        CancellationToken cancellationToken
    );
    Task<DragonSetLockResponse> DoDragonSetLock(
        DragonSetLockRequest request,
        CancellationToken cancellationToken
    );
    Task<DragonSellResponse> DoDragonSell(
        DragonSellRequest request,
        CancellationToken cancellationToken
    );
    Task<int> GetFreeGiftCount();
}
