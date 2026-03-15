using System.Collections.Generic;
using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Serilog.Context;

namespace DragaliaAPI.Features.Weapons;

public partial class WeaponService : IWeaponService
{
    private readonly IWeaponRepository weaponRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IFortRepository fortRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<WeaponService> logger;
    private readonly IMissionProgressionService missionProgressionService;

    public WeaponService(
        IWeaponRepository weaponRepository,
        IInventoryRepository inventoryRepository,
        IFortRepository fortRepository,
        IUserDataRepository userDataRepository,
        ILogger<WeaponService> logger,
        IMissionProgressionService missionProgressionService
    )
    {
        this.weaponRepository = weaponRepository;
        this.inventoryRepository = inventoryRepository;
        this.fortRepository = fortRepository;
        this.userDataRepository = userDataRepository;
        this.logger = logger;
        this.missionProgressionService = missionProgressionService;
    }

    public async Task<bool> ValidateCraft(WeaponBodies weaponBodyId)
    {
        if (await this.weaponRepository.CheckOwnsWeapons(weaponBodyId))
        {
            Log.PlayerAlreadyOwnsWeapon(this.logger, weaponBodyId);
            return false;
        }

        WeaponBody weaponData = MasterAsset.WeaponBody.Get(weaponBodyId);

        if (
            !await this.fortRepository.CheckPlantLevel(
                FortPlants.Smithy,
                weaponData.NeedFortCraftLevel
            )
        )
        {
            Log.PlayerSmithyLevelWasTooLowToCraftWeaponNeedsLevel(
                this.logger,
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
            Log.PlayerDidNotHaveOneOrMoreWeaponsRequiredToCraft(
                this.logger,
                weaponData.NeedCreateWeaponBodyId1,
                weaponData.NeedCreateWeaponBodyId2,
                weaponBodyId
            );
            return false;
        }

        // TODO: _NeedAllUnlockWeaponBodyId1

        if (!await this.inventoryRepository.CheckQuantity(weaponData.CreateMaterialMap))
        {
            Log.PlayerLackedMaterialsToCraftWeapon(this.logger, weaponBodyId);
            return false;
        }

        if (!await this.userDataRepository.CheckCoin(weaponData.CreateCoin))
        {
            Log.PlayerLackedRupiesToCraftWeapon(this.logger, weaponBodyId);
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

        await this.userDataRepository.UpdateCoin(-weaponData.CreateCoin);

        await this.weaponRepository.Add(weaponBodyId);
        await this.weaponRepository.AddSkin((int)weaponBodyId);

        this.missionProgressionService.OnWeaponEarned(
            weaponBodyId,
            weaponData.ElementalType,
            weaponData.Rarity,
            weaponData.WeaponSeriesId
        );
    }

    public async Task<bool> CheckOwned(WeaponBodies weaponBodyId) =>
        await this.weaponRepository.CheckOwnsWeapons(weaponBodyId);

    /// <summary>
    /// Try to buildup a weapon.
    /// </summary>
    /// <param name="body">MasterAsset weapon body data</param>
    /// <param name="buildup"></param>
    /// <returns></returns>
    public async Task<ResultCode> TryBuildup(
        WeaponBody body,
        AtgenBuildupWeaponBodyPieceList buildup
    )
    {
        LogContext.PushProperty("WeaponBodyId", body.Id);
        LogContext.PushProperty("BuildupRequest", buildup, destructureObjects: true);

        return buildup.BuildupPieceType switch
        {
            BuildupPieceTypes.Passive => await this.TryBuildupPassive(body, buildup),
            BuildupPieceTypes.Stats => await this.TryBuildupStats(body, buildup),
            _ => await this.TryBuildupGeneric(body, buildup),
        };
    }

    /// <summary>
    /// Try to buildup a weapon for any other buildup type.
    /// </summary>
    /// <param name="body">MasterAsset weapon body data.</param>
    /// <param name="buildup">Buildup request.</param>
    /// <returns>A ResultCode which is Success if the request was processed, or otherwise some kind of error.</returns>
    private async Task<ResultCode> TryBuildupGeneric(
        WeaponBody body,
        AtgenBuildupWeaponBodyPieceList buildup
    )
    {
        // Possibly would be more readable if validation was done in a separate method, but it would require duplicating
        // a fair bit of logic, e.g. around getting the materials (first to validate them, then to deduct them).
        int buildupKey = body.GetBuildupGroupId(buildup.BuildupPieceType, buildup.Step);

        // TODO: Use of WeaponBodyRarity table to validate unbind/stat upgrades against refine count

        if (
            !MasterAsset.WeaponBodyBuildupGroup.TryGetValue(
                buildupKey,
                out WeaponBodyBuildupGroup? buildupGroup
            )
        )
        {
            Log.CouldNotFindBuildupGroupWithKey(this.logger, buildupKey);
            return ResultCode.WeaponBodyBuildupPieceUnablePiece;
        }

        Dictionary<Materials, int> materialMap = buildupGroup.MaterialMap.ToDictionary();
        long coin = buildupGroup.BuildupCoin;

        if (buildup is { BuildupPieceType: BuildupPieceTypes.Unbind, IsUseDedicatedMaterial: true })
        {
            SetMaterialMapSpecial(body, ref materialMap, ref coin);
            Log.UsingSpecialMaterialMaterialMapSetTo(this.logger, materialMap);
        }

        if (!await this.ValidateCost(materialMap, coin))
            return ResultCode.CommonMaterialShort;

        DbWeaponBody? entity = await this.weaponRepository.FindAsync(body.Id);
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.LimitBreakCount < buildupGroup.UnlockConditionLimitBreakCount)
        {
            Log.EntityWithLimitBreakCountWasIneligibleForBuildupGroupWithMinLimitBreakCount(
                this.logger,
                entity.LimitBreakCount,
                buildupGroup.UnlockConditionLimitBreakCount
            );

            return ResultCode.WeaponBodyBuildupPieceShortLimitBreakCount;
        }

        switch (buildup.BuildupPieceType)
        {
            case BuildupPieceTypes.Copies:
                if (!this.ValidateStep(entity.EquipableCount, buildup))
                    return ResultCode.WeaponBodyBuildupPieceStepError;
                entity.EquipableCount = buildup.Step;
                break;
            case BuildupPieceTypes.Unbind:
                if (!this.ValidateStep(entity.LimitBreakCount, buildup))
                    return ResultCode.WeaponBodyBuildupPieceStepError;
                entity.LimitBreakCount = buildup.Step;
                break;
            case BuildupPieceTypes.Refine:
                if (!this.ValidateStep(entity.LimitOverCount, buildup))
                    return ResultCode.WeaponBodyBuildupPieceStepError;
                entity.LimitOverCount = buildup.Step;
                this.missionProgressionService.OnWeaponRefined(
                    1,
                    buildup.Step,
                    body.Id,
                    body.ElementalType,
                    body.Rarity,
                    body.WeaponSeriesId
                );
                break;
            case BuildupPieceTypes.WeaponBonus:
                if (!this.ValidateStep(entity.FortPassiveCharaWeaponBuildupCount, buildup))
                    return ResultCode.WeaponBodyBuildupPieceStepError;
                entity.FortPassiveCharaWeaponBuildupCount = buildup.Step;
                break;
            case BuildupPieceTypes.CrestSlotType1:
                if (!this.ValidateStep(entity.AdditionalCrestSlotType1Count, buildup))
                    return ResultCode.WeaponBodyBuildupPieceStepError;
                entity.AdditionalCrestSlotType1Count = buildup.Step;
                break;
            case BuildupPieceTypes.CrestSlotType3:
                if (!this.ValidateStep(entity.AdditionalCrestSlotType3Count, buildup))
                    return ResultCode.WeaponBodyBuildupPieceStepError;
                entity.AdditionalCrestSlotType3Count = buildup.Step;
                break;
            case BuildupPieceTypes.Stats:
            case BuildupPieceTypes.Passive:
                throw new UnreachableException("Received non-generic buildup_piece_type");
            default:
                Log.BuildupPieceTypeHadInvalidValue(this.logger, buildup.BuildupPieceType);
                return ResultCode.CommonInvalidArgument;
        }

        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());
        await this.userDataRepository.UpdateCoin(-coin);
        await this.AddRewardWeaponSkins(buildupGroup, body);

        return ResultCode.Success;
    }

    /// <summary>
    /// Attempt to buildup a weapon's passive ability.
    /// </summary>
    /// <param name="body">MasterAsset weapon body data.</param>
    /// <param name="buildup">The buildup request.</param>
    /// <returns>A ResultCode which is Success if the request was processed, or otherwise some kind of error.</returns>
    private async Task<ResultCode> TryBuildupPassive(
        WeaponBody body,
        AtgenBuildupWeaponBodyPieceList buildup
    )
    {
        int passiveKey = body.GetPassiveAbilityId(buildup.BuildupPieceNo);

        if (
            !MasterAsset.WeaponPassiveAbility.TryGetValue(
                passiveKey,
                out WeaponPassiveAbility? passiveAbility
            )
        )
        {
            Log.InvalidWeaponPassiveAbilityKey(this.logger, passiveKey);
            return ResultCode.WeaponBodyBuildupPieceUnablePiece;
        }

        Dictionary<Materials, int> materialMap = passiveAbility.MaterialMap.ToDictionary();
        long coin = passiveAbility.UnlockCoin;

        if (!await this.ValidateCost(materialMap, coin))
            return ResultCode.CommonMaterialShort;

        DbWeaponBody? entity = await this.weaponRepository.FindAsync(body.Id);
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.LimitBreakCount < passiveAbility.UnlockConditionLimitBreakCount)
        {
            Log.EntityWithLimitBreakCountWasIneligibleForBuildupGroupWithMinLimitBreakCount(
                this.logger,
                entity.LimitBreakCount,
                buildupGroup.UnlockConditionLimitBreakCount
            );

            return ResultCode.WeaponBodyBuildupPieceShortLimitBreakCount;
        }

        await this.weaponRepository.AddPassiveAbility(entity.WeaponBodyId, passiveAbility);
        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());
        await this.userDataRepository.UpdateCoin(-coin);
        await this.AddRewardWeaponSkins(passiveAbility);

        return ResultCode.Success;
    }

    /// <summary>
    /// Attempt to buildup a weapon's stats.
    /// </summary>
    /// <param name="body">MasterAsset weapon body data.</param>
    /// <param name="buildup">The buildup request.</param>
    /// <returns>A ResultCode which is Success if the request was processed, or otherwise some kind of error.</returns>
    private async Task<ResultCode> TryBuildupStats(
        WeaponBody body,
        AtgenBuildupWeaponBodyPieceList buildup
    )
    {
        int passiveKey = body.GetBuildupLevelId(buildup.Step);

        if (
            !MasterAsset.WeaponBodyBuildupLevel.TryGetValue(
                passiveKey,
                out WeaponBodyBuildupLevel? buildupLevel
            )
        )
        {
            Log.InvalidWeaponStatBuildupKey(this.logger, passiveKey);
            return ResultCode.WeaponBodyBuildupPieceUnablePiece;
        }

        Dictionary<Materials, int> materialMap = buildupLevel.MaterialMap.ToDictionary();

        if (!await this.ValidateCost(materialMap))
            return ResultCode.CommonMaterialShort;

        DbWeaponBody? entity = await this.weaponRepository.FindAsync(body.Id);
        ArgumentNullException.ThrowIfNull(entity);

        if (!this.ValidateStep(entity.BuildupCount, buildup))
            return ResultCode.WeaponBodyBuildupPieceStepError;

        entity.BuildupCount = buildup.Step;
        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());

        return ResultCode.Success;
    }

    /// <summary>
    /// Try to validate coin and material cost.
    /// </summary>
    /// <param name="materialMap">Material cost map.</param>
    /// <param name="coin">Coin cost.</param>
    /// <returns>A bool indicating whether the user has passed the check.</returns>
    private async Task<bool> ValidateCost(Dictionary<Materials, int> materialMap, long coin)
    {
        if (!await this.ValidateCost(materialMap))
            return false;

        if (!await this.userDataRepository.CheckCoin(coin))
        {
            Log.PlayerHadInsufficientRupiesToUpgradeWeapon(this.logger);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Try to validate material cost.
    /// </summary>
    /// <param name="materialMap">Material cost map.</param>
    /// <returns>A bool indicating whether the user has passed the check.</returns>
    private async Task<bool> ValidateCost(Dictionary<Materials, int> materialMap)
    {
        if (!await this.inventoryRepository.CheckQuantity(materialMap))
        {
            Log.PlayerHadInsufficientMaterialsToUpgradeWeapon(this.logger);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validate a step for a weapon entity property.
    /// Steps should always be performed in increments of 1.
    /// </summary>
    /// <param name="entityProperty">The integer property of the entity.</param>
    /// <param name="buildup">The buildup request.</param>
    /// <returns>A bool indicating whether the request was valid.</returns>
    private bool ValidateStep(int entityProperty, AtgenBuildupWeaponBodyPieceList buildup)
    {
        if (entityProperty != buildup.Step - 1)
        {
            Log.WeaponPropertyValueWasInInvalidStateForBuildup(
                this.logger,
                entityProperty,
                buildup
            );
            return false;
        }

        return true;
    }

    /// <summary>
    /// Reassign the material map and coin values for the case where a dedicated material (e.g. damascus ingot) is used.
    /// </summary>
    /// <param name="body">MasterAsset weapon body data.</param>
    /// <param name="materialMap">The old material map.</param>
    /// <param name="coin">The old coin cost.</param>
    /// <exception cref="DragaliaException">The weapon had an invalid rarity (higher than 6; lower than 4)</exception>
    private static void SetMaterialMapSpecial(
        WeaponBody body,
        ref Dictionary<Materials, int> materialMap,
        ref long coin
    )
    {
        Materials mat = body.Rarity switch
        {
            6 => Materials.AdamantiteIngot,
            5 => Materials.DamascusIngot,
            4 => Materials.SteelBrick,
            _ => throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Invalid dedicated material rarity"
            ),
        };

        materialMap = new Dictionary<Materials, int>() { { mat, 1 } };
        coin = 0;
    }

    /// <summary>
    /// Add reward weapon skins for a generic buildup.
    /// </summary>
    /// <param name="buildupGroup">The buildup group.</param>
    /// <param name="body">The MasterAsset weapon body data.</param>
    /// <exception cref="UnreachableException">The buildup group had an invalid RewardWeaponSkinNo.</exception>
    private async Task AddRewardWeaponSkins(WeaponBodyBuildupGroup buildupGroup, WeaponBody body)
    {
        switch (buildupGroup.RewardWeaponSkinNo)
        {
            case 0:
                break;
            case 1:
                await this.weaponRepository.AddSkin(body.RewardWeaponSkinId1);
                break;
            case 2:
                await this.weaponRepository.AddSkin(body.RewardWeaponSkinId2);
                break;
            case 3:
                await this.weaponRepository.AddSkin(body.RewardWeaponSkinId3);
                break;
            case 4:
                // Never used
                await this.weaponRepository.AddSkin(body.RewardWeaponSkinId4);
                break;
            case 5:
                // Never used
                await this.weaponRepository.AddSkin(body.RewardWeaponSkinId5);
                break;
            default:
                throw new UnreachableException("Invalid RewardSkinWeaponNo");
        }
    }

    /// <summary>
    /// Add reward weapon skins for a passive ability unlock.
    /// </summary>
    /// <param name="passiveAbility">The passive ability.</param>
    private async Task AddRewardWeaponSkins(WeaponPassiveAbility passiveAbility)
    {
        if (passiveAbility.RewardWeaponSkinId1 != 0)
            await this.weaponRepository.AddSkin(passiveAbility.RewardWeaponSkinId1);

        if (passiveAbility.RewardWeaponSkinId2 != 0)
            await this.weaponRepository.AddSkin(passiveAbility.RewardWeaponSkinId2);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Player already owns weapon {weapon}")]
        public static partial void PlayerAlreadyOwnsWeapon(ILogger logger, WeaponBodies weapon);

        [LoggerMessage(
            LogLevel.Warning,
            "Player smithy level was too low to craft weapon {weapon} (needs level {level2})"
        )]
        public static partial void PlayerSmithyLevelWasTooLowToCraftWeaponNeedsLevel(
            ILogger logger,
            WeaponBodies weapon,
            int level2
        );

        [LoggerMessage(
            LogLevel.Warning,
            "Player did not have one or more weapons ({weapon1}, {weapon2}) required to craft {weapon3}"
        )]
        public static partial void PlayerDidNotHaveOneOrMoreWeaponsRequiredToCraft(
            ILogger logger,
            WeaponBodies weapon1,
            WeaponBodies weapon2,
            WeaponBodies weapon3
        );

        [LoggerMessage(LogLevel.Warning, "Player lacked materials to craft weapon {weapon}")]
        public static partial void PlayerLackedMaterialsToCraftWeapon(
            ILogger logger,
            WeaponBodies weapon
        );

        [LoggerMessage(LogLevel.Warning, "Player lacked rupies to craft weapon {weapon}")]
        public static partial void PlayerLackedRupiesToCraftWeapon(
            ILogger logger,
            WeaponBodies weapon
        );

        [LoggerMessage(LogLevel.Error, "Could not find buildup group with key {key}")]
        public static partial void CouldNotFindBuildupGroupWithKey(ILogger logger, int key);

        [LoggerMessage(LogLevel.Information, "Using special material; material map set to {map}")]
        public static partial void UsingSpecialMaterialMaterialMapSetTo(
            ILogger logger,
            Dictionary<Materials, int> map
        );

        [LoggerMessage(
            LogLevel.Error,
            "Entity with limit break count {count} was ineligible for buildupGroup with min limit break count {count2}"
        )]
        public static partial void EntityWithLimitBreakCountWasIneligibleForBuildupGroupWithMinLimitBreakCount(
            ILogger logger,
            int count,
            int count2
        );

        [LoggerMessage(LogLevel.Warning, "buildup_piece_type had invalid value: {type}")]
        public static partial void BuildupPieceTypeHadInvalidValue(
            ILogger logger,
            BuildupPieceTypes type
        );

        [LoggerMessage(LogLevel.Error, "Invalid weapon passive ability key {key}")]
        public static partial void InvalidWeaponPassiveAbilityKey(ILogger logger, int key);

        [LoggerMessage(LogLevel.Error, "Invalid weapon stat buildup key {key}")]
        public static partial void InvalidWeaponStatBuildupKey(ILogger logger, int key);

        [LoggerMessage(LogLevel.Warning, "Player had insufficient rupies to upgrade weapon")]
        public static partial void PlayerHadInsufficientRupiesToUpgradeWeapon(ILogger logger);

        [LoggerMessage(LogLevel.Warning, "Player had insufficient materials to upgrade weapon")]
        public static partial void PlayerHadInsufficientMaterialsToUpgradeWeapon(ILogger logger);

        [LoggerMessage(
            LogLevel.Warning,
            "Weapon property value {value} was in invalid state for buildup {@buildup}"
        )]
        public static partial void WeaponPropertyValueWasInInvalidStateForBuildup(
            ILogger logger,
            int value,
            AtgenBuildupWeaponBodyPieceList buildup
        );
    }
}
