using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class WeaponService : IWeaponService
{
    private readonly IWeaponRepository weaponRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IFortRepository fortRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<WeaponService> logger;

    public WeaponService(
        IWeaponRepository weaponRepository,
        IInventoryRepository inventoryRepository,
        IFortRepository fortRepository,
        IUserDataRepository userDataRepository,
        ILogger<WeaponService> logger
    )
    {
        this.weaponRepository = weaponRepository;
        this.inventoryRepository = inventoryRepository;
        this.fortRepository = fortRepository;
        this.userDataRepository = userDataRepository;
        this.logger = logger;
    }

    public async Task<bool> ValidateCraft(WeaponBodies weaponBodyId)
    {
        if (await this.weaponRepository.CheckOwnsWeapons(weaponBodyId))
        {
            this.logger.LogDebug("Player already owns weapon {weapon}", weaponBodyId);
            return false;
        }

        WeaponBody weaponData = MasterAsset.WeaponBody.Get(weaponBodyId);

        if (!await fortRepository.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel))
        {
            this.logger.LogDebug(
                "Player smithy level was too low to craft weapon {weapon} (needs level {level2})",
                weaponBodyId,
                weaponData.NeedFortCraftLevel
            );
            return false;
        }

        if (
            !await this.weaponRepository.CheckOwnsWeapons(
                weaponData.NeedCreateWeaponBodyId1,
                weaponData.NeedCreateWeaponBodyId2
            )
        )
        {
            this.logger.LogDebug(
                "Player did not have one or more weapons ({weapon1}, {weapon2}) required to craft {weapon3}",
                weaponData.NeedCreateWeaponBodyId1,
                weaponData.NeedCreateWeaponBodyId2,
                weaponBodyId
            );
            return false;
        }

        // TODO: _NeedAllUnlockWeaponBodyId1

        if (!await this.inventoryRepository.CheckQuantity(weaponData.CreateMaterialMap))
        {
            this.logger.LogDebug("Player lacked materials to craft weapon {weapon}", weaponBodyId);
            return false;
        }

        if (!await this.userDataRepository.CheckCoin(weaponData.CreateCoin))
        {
            this.logger.LogDebug("Player lacked rupies to craft weapon {weapon}", weaponBodyId);
            return false;
        }

        return true;
    }

    public async Task Craft(WeaponBodies weaponBodyId)
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(weaponBodyId);

        await this.inventoryRepository.UpdateQuantity(
            weaponData.CreateMaterialMap.ToDictionary(x => x.Key, x => -x.Value)
        );

        await this.userDataRepository.UpdateRupies(-weaponData.CreateCoin);

        await this.weaponRepository.Add(weaponBodyId);
        await this.weaponRepository.AddSkin((int)weaponBodyId);
    }
}
