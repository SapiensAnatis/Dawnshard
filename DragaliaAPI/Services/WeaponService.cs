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

    public async Task<bool> ValidateCraft(string accountId, WeaponBodies weaponBodyId)
    {
        if (await this.CheckOwnsWeapons(accountId, weaponBodyId))
        {
            this.logger.LogWarning("User already owns weapon {weapon}", weaponBodyId);
            return false;
        }

        WeaponBody weaponData = MasterAsset.WeaponBody.Get(weaponBodyId);

        int smithyLevel = await this.GetSmithyLevel(accountId);
        if (smithyLevel < weaponData.NeedFortCraftLevel)
        {
            this.logger.LogWarning(
                "Player smithy level {level1} was too low to craft weapon {weapon} (needs level {level2})",
                smithyLevel,
                weaponBodyId,
                weaponData.NeedFortCraftLevel
            );
            return false;
        }

        if (
            !await this.CheckOwnsWeapons(
                accountId,
                weaponData.NeedCreateWeaponBodyId1,
                weaponData.NeedCreateWeaponBodyId2
            )
        )
        {
            this.logger.LogWarning(
                "Player did not have one or more weapons ({weapon1}, {weapon2}) required to craft {weapon3}",
                weaponData.NeedCreateWeaponBodyId1,
                weaponData.NeedCreateWeaponBodyId2,
                weaponBodyId
            );
            return false;
        }

        // TODO: _NeedAllUnlockWeaponBodyId1

        if (
            !(
                await this.inventoryRepository.CheckHasMaterialQuantity(
                    accountId,
                    weaponData.QuantityMap
                )
            )
        )
        {
            this.logger.LogWarning(
                "Player lacked materials to craft weapon {weapon}",
                weaponBodyId
            );
            return false;
        }

        long coin = (await this.userDataRepository.LookupUserData(accountId)).Coin;
        if (coin < weaponData.CreateCoin)
        {
            this.logger.LogWarning(
                "Player had too few rupies ({rupies}) to craft weapon {weapon} (needs {neededRupies})",
                coin,
                weaponBodyId,
                weaponData.CreateCoin
            );
        }

        return true;
    }

    public async Task Craft(string accountId, WeaponBodies weaponBodyId)
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(weaponBodyId);

        await this.inventoryRepository.AddMaterialQuantity(
            accountId,
            weaponData.CreateEntityId1,
            -weaponData.CreateEntityQuantity1
        );

        await this.inventoryRepository.AddMaterialQuantity(
            accountId,
            weaponData.CreateEntityId2,
            -weaponData.CreateEntityQuantity2
        );

        await this.inventoryRepository.AddMaterialQuantity(
            accountId,
            weaponData.CreateEntityId3,
            -weaponData.CreateEntityQuantity3
        );

        await this.inventoryRepository.AddMaterialQuantity(
            accountId,
            weaponData.CreateEntityId4,
            -weaponData.CreateEntityQuantity4
        );

        await this.inventoryRepository.AddMaterialQuantity(
            accountId,
            weaponData.CreateEntityId5,
            -weaponData.CreateEntityQuantity5
        );

        await this.userDataRepository.UpdateRupies(accountId, -weaponData.CreateCoin);

        await this.weaponRepository.Add(accountId, weaponBodyId);
        await this.weaponRepository.AddSkin(accountId, (int)weaponBodyId);
    }

    private async Task<int> GetSmithyLevel(string accountId)
    {
        return await this.fortRepository
            .GetBuilds(accountId)
            .Where(x => x.PlantId == FortPlants.Smithy)
            .Select(x => x.Level)
            .FirstOrDefaultAsync();
    }

    private async Task<bool> CheckOwnsWeapons(string accountId, params WeaponBodies[] weaponIds)
    {
        List<WeaponBodies> filtered = weaponIds.Where(x => x != WeaponBodies.Empty).ToList();
        if (!filtered.Any())
            return true;

        return (
                await this.weaponRepository
                    .GetWeaponBodies(accountId)
                    .Select(x => x.WeaponBodyId)
                    .Where(x => weaponIds.Contains(x))
                    .ToListAsync()
            )
                .Intersect(filtered)
                .Count() == filtered.Count;
    }
}
