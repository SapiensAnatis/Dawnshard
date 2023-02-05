using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
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

    public async Task<bool> ValidateBuildup(
        WeaponBodies id,
        AtgenBuildupWeaponBodyPieceList buildup
    )
    {
        Dictionary<Materials, int> materialMap = GetMaterials(
            MasterAsset.WeaponBody.Get(id),
            buildup
        );

        if (!await this.inventoryRepository.CheckQuantity(materialMap))
        {
            this.logger.LogDebug("Player had insufficient materials to upgrade weapon");
            return false;
        }

        if (await this.weaponRepository.FindAsync(id) is null)
        {
            this.logger.LogDebug("Player did not own weapon {weapon}", id);
            return false;
        }

        // Could also validate that the weapon is in the correct state for the action,
        // i.e. can't go from 2 unbinds to 8, but I'm lazy

        return true;
    }

    public async Task UnlockBuildup(WeaponBodies id, AtgenBuildupWeaponBodyPieceList buildup)
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(id);

        Dictionary<Materials, int> materialMap = GetMaterials(weaponData, buildup);

        DbWeaponBody weapon =
            await this.weaponRepository.FindAsync(id)
            ?? throw new InvalidOperationException("Could not find weapon");

        switch (buildup.buildup_piece_type)
        {
            case BuildupPieceTypes.Stats:
                weapon.BuildupCount = buildup.step;
                break;
            case BuildupPieceTypes.Unbind:
                weapon.LimitBreakCount = buildup.step;
                break;
            case BuildupPieceTypes.WeaponBonus:
                weapon.FortPassiveCharaWeaponBuildupCount = buildup.step;
                break;
            case BuildupPieceTypes.Copies:
                weapon.EquipableCount = buildup.step;
                break;
            case BuildupPieceTypes.Refine:
                weapon.LimitOverCount = buildup.step;
                break;
            case BuildupPieceTypes.CrestSlotType1:
                // buildup.step is always 1
                weapon.AdditionalCrestSlotType1Count = buildup.step;
                break;
            case BuildupPieceTypes.CrestSlotType3:
                // buildup.step is 1 or 2 depending on which slot is being unlocked
                weapon.AdditionalCrestSlotType3Count = buildup.step;
                break;
            case BuildupPieceTypes.Passive:
                await this.weaponRepository.AddPassiveAbility(id, buildup.buildup_piece_no);
                break;
            default:
                throw new ArgumentException($"Invalid buildup type {buildup.buildup_piece_type}!");
        }

        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());
        await this.userDataRepository.UpdateRupies(-GetRupies(weaponData, buildup));
    }

    private static Dictionary<Materials, int> GetMaterials(
        WeaponBody body,
        AtgenBuildupWeaponBodyPieceList buildup
    )
    {
        if (buildup.is_use_dedicated_material)
        {
            Materials mat = body.Rarity switch
            {
                6 => Materials.AdamantiteIngot,
                5 => Materials.DamascusIngot,
                4 => Materials.SteelBrick,
                _
                    => throw new DragaliaException(
                        ResultCode.WeaponBodyBuildupPieceUnablePiece,
                        "Invalid dedicated material use!"
                    )
            };

            return new Dictionary<Materials, int>() { { mat, 1 } };
        }

        return buildup.buildup_piece_type switch
        {
            BuildupPieceTypes.Passive
                => body.GetPassiveAbility(buildup.buildup_piece_no).MaterialMap,
            BuildupPieceTypes.Stats => body.GetBuildupLevel(buildup.step).MaterialMap,
            _ => body.GetBuildupGroup(buildup.buildup_piece_type, buildup.step).MaterialMap
        };
    }

    private static long GetRupies(WeaponBody body, AtgenBuildupWeaponBodyPieceList buildup)
    {
        if (buildup.is_use_dedicated_material)
            return 0;

        return buildup.buildup_piece_type switch
        {
            BuildupPieceTypes.Stats => 0,
            BuildupPieceTypes.Passive
                => body.GetPassiveAbility(buildup.buildup_piece_no).UnlockCoin,
            _ => body.GetBuildupGroup(buildup.buildup_piece_type, buildup.step).BuildupCoin
        };
    }
}
