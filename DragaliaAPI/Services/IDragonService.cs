using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IDragonService
{
    Task<DragonBuyGiftToSendMultipleData> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request,
        string deviceAccountId
    );
    Task<DragonSendGiftMultipleData> DoDragonSendGiftMultiple(
        DragonSendGiftMultipleRequest request,
        string deviceAccountId
    );
    Task<DragonGetContactDataData> DoDragonGetContactData(
        DragonGetContactDataRequest request,
        string deviceAccountId
    );
    Task<DragonBuildupData> DoBuildup(DragonBuildupRequest request, string deviceAccountId);
    Task<DragonResetPlusCountData> DoDragonResetPlusCount(
        DragonResetPlusCountRequest request,
        string deviceAccountId
    );
    Task<DragonLimitBreakData> DoDragonLimitBreak(
        DragonLimitBreakRequest request,
        string deviceAccountId
    );
    Task<DragonSetLockData> DoDragonSetLock(DragonSetLockRequest request, string deviceAccountId);
    Task<DragonSellData> DoDragonSell(DragonSellRequest request, string deviceAccountId);
}
