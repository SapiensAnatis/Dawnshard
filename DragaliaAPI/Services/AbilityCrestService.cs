using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Serilog.Context;

namespace DragaliaAPI.Services;

public class AbilityCrestService : IAbilityCrestService
{
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<WeaponService> logger;

    public AbilityCrestService(
        IAbilityCrestRepository abilityCrestRepository,
        IInventoryRepository inventoryRepository,
        IUserDataRepository userDataRepository,
        ILogger<WeaponService> logger
    )
    {
        this.abilityCrestRepository = abilityCrestRepository;
        this.inventoryRepository = inventoryRepository;
        this.userDataRepository = userDataRepository;
        this.logger = logger;
    }

    public async Task AddOrRefund(AbilityCrests abilityCrestId)
    {
        // for if wyrmprints get added to quest drops in future

        DbAbilityCrest? abilityCrest = await this.abilityCrestRepository.FindAsync(abilityCrestId);

        if (abilityCrest is null)
        {
            await this.abilityCrestRepository.Add(abilityCrestId);
        }
        else
        {
            this.logger.LogDebug("Ability crest already owned, refunding materials instead");
            AbilityCrest abilityCrestInfo = MasterAsset.AbilityCrest.Get(abilityCrestId);

            switch (abilityCrestInfo.DuplicateEntityType)
            {
                case EntityTypes.Material:
                    await this.inventoryRepository.UpdateQuantity(
                        abilityCrestInfo.DuplicateMaterialMap.Invert()
                    );
                    break;
                case EntityTypes.Dew:
                    await this.userDataRepository.UpdateDewpoint(
                        -abilityCrestInfo.DuplicateEntityQuantity
                    );
                    break;
                case EntityTypes.Rupies:
                    await this.userDataRepository.UpdateCoin(
                        -abilityCrestInfo.DuplicateEntityQuantity
                    );
                    break;
                default:
                    this.logger.LogWarning(
                        "Ability crest DuplicateEntityType {type} invalid",
                        abilityCrestInfo.DuplicateEntityType
                    );
                    return;
            }
        }
    }
}
