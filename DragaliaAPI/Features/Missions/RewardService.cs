using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class RewardService : IRewardService
{
    private readonly ILogger<RewardService> logger;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUnitRepository unitRepository;

    public RewardService(
        ILogger<RewardService> logger,
        IInventoryRepository inventoryRepository,
        IUserDataRepository userDataRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUnitRepository unitRepository
    )
    {
        this.logger = logger;
        this.inventoryRepository = inventoryRepository;
        this.userDataRepository = userDataRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.unitRepository = unitRepository;
    }

    public async Task GrantReward(
        EntityTypes type,
        int id,
        int quantity = 1,
        int limitBreakCount = -1,
        int buildupCount = -1,
        int equippableCount = -1
    )
    {
        switch (type)
        {
            case EntityTypes.Chara:
                await this.unitRepository.AddCharas((Charas)id);
                break;
            case EntityTypes.Dragon:
                await this.unitRepository.AddDragons((Dragons)id);
                break;
            case EntityTypes.Dew:
                await this.userDataRepository.UpdateDewpoint(quantity);
                break;
            case EntityTypes.HustleHammer:
                (await this.userDataRepository.UserData.SingleAsync()).BuildTimePoint += quantity;
                break;
            case EntityTypes.Rupies:
                await this.userDataRepository.UpdateCoin(quantity);
                break;
            case EntityTypes.Wyrmite:
                await this.userDataRepository.GiveWyrmite(quantity);
                break;
            case EntityTypes.Wyrmprint:
                if (limitBreakCount == -1 || buildupCount == -1 || equippableCount == -1)
                {
                    throw new InvalidOperationException(
                        "Invalid parameters for granting wyrmprint."
                    );
                }

                // TODO?
                throw new NotImplementedException();
                break;
            case EntityTypes.Material:
                (
                    await this.inventoryRepository.GetMaterial((Materials)id)
                    ?? this.inventoryRepository.AddMaterial((Materials)id)
                ).Quantity += quantity;
                break;
            default:
                logger.LogWarning(
                    "Tried to reward unsupported entity type {rewardEntityType}. Parameters: {@parameters}",
                    type,
                    new
                    {
                        type,
                        id,
                        quantity,
                        limitBreakCount,
                        buildupCount,
                        equippableCount
                    }
                );
                break;
        }
    }
}
