using DragaliaAPI.Database.Repositories;

namespace DragaliaAPI.Features.Login;

public class DragonGiftResetAction : IDailyResetAction
{
    private readonly IInventoryRepository inventoryRepository;

    public DragonGiftResetAction(IInventoryRepository inventoryRepository)
    {
        this.inventoryRepository = inventoryRepository;
    }

    public async Task Apply()
    {
        await this.inventoryRepository.RefreshPurchasableDragonGiftCounts();
    }
}
