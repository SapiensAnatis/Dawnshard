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
}
