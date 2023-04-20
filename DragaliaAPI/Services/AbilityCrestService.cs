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
    private readonly ILogger<AbilityCrestService> logger;

    public AbilityCrestService(
        IAbilityCrestRepository abilityCrestRepository,
        IInventoryRepository inventoryRepository,
        IUserDataRepository userDataRepository,
        ILogger<AbilityCrestService> logger
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
                        abilityCrestInfo.DuplicateMaterialMap
                    );
                    break;
                case EntityTypes.Dew:
                    await this.userDataRepository.UpdateDewpoint(
                        abilityCrestInfo.DuplicateEntityQuantity
                    );
                    break;
                case EntityTypes.Rupies:
                    await this.userDataRepository.UpdateCoin(
                        abilityCrestInfo.DuplicateEntityQuantity
                    );
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.CommonInvalidArgument,
                        $"DuplicateEntityType {abilityCrestInfo.DuplicateEntityType} invalid"
                    );
            }
        }
    }

    public async Task<ResultCode> TryBuildup(
        AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    )
    {
        switch (buildup.buildup_piece_type)
        {
            case BuildupPieceTypes.Unbind
            or BuildupPieceTypes.Copies:
                return await TryBuildupGeneric(abilityCrest, buildup);
            case BuildupPieceTypes.Stats:
                return await TryBuildupLevel(abilityCrest, buildup);
            default:
                this.logger.LogWarning(
                    "Buildup piece type {type} invalid",
                    buildup.buildup_piece_type
                );
                return ResultCode.CommonInvalidArgument;
        }
    }

    private async Task<ResultCode> TryBuildupGeneric(
        AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    )
    {
        int buildupId = abilityCrest.GetBuildupGroupId(buildup.buildup_piece_type, buildup.step);

        if (
            !MasterAsset.AbilityCrestBuildupGroup.TryGetValue(
                buildupId,
                out AbilityCrestBuildupGroup? buildupInfo
            )
        )
        {
            this.logger.LogWarning("BuildupGroupId {id} invalid", buildupId);
            return ResultCode.AbilityCrestBuildupPieceUnablePiece;
        }

        Dictionary<Materials, int> materialMap = buildupInfo.MaterialMap;
        int dewpoint = buildupInfo.BuildupDewPoint;

        if (buildupInfo.IsUseUniqueMaterial)
        {
            materialMap[abilityCrest.UniqueBuildupMaterialId] =
                buildupInfo.UniqueBuildupMaterialCount;
        }

        SetMaterialMapSpecial(abilityCrest.Rarity, buildup, ref materialMap, ref dewpoint);

        if (!(await ValidateCost(materialMap) && await ValidateCost(dewpoint)))
        {
            return ResultCode.AbilityCrestBuildupPieceStepError;
        }

        DbAbilityCrest dbAbilityCrest = await TryFindAsync(abilityCrest.Id);

        if (buildup.buildup_piece_type == BuildupPieceTypes.Unbind)
        {
            if (!ValidateStep(dbAbilityCrest.LimitBreakCount, buildup.step))
            {
                return ResultCode.AbilityCrestBuildupPieceStepError;
            }

            dbAbilityCrest.LimitBreakCount = buildup.step;
        }
        else
        {
            if (!ValidateStep(dbAbilityCrest.EquipableCount, buildup.step))
            {
                return ResultCode.AbilityCrestBuildupPieceStepError;
            }

            dbAbilityCrest.EquipableCount = buildup.step;
        }

        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());
        await this.userDataRepository.UpdateDewpoint(-dewpoint);

        return ResultCode.Success;
    }

    private async Task<ResultCode> TryBuildupLevel(
        AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    )
    {
        int levelId = abilityCrest.GetBuildupLevelId(buildup.step);

        if (
            !MasterAsset.AbilityCrestBuildupLevel.TryGetValue(
                levelId,
                out AbilityCrestBuildupLevel? levelInfo
            )
        )
        {
            this.logger.LogWarning("BuildupLevelId {id} invalid", levelId);
            return ResultCode.AbilityCrestBuildupPieceUnablePiece;
        }

        CheckDedicated(buildup);
        Dictionary<Materials, int> materialMap = levelInfo.MaterialMap;

        if (levelInfo.IsUseUniqueMaterial)
        {
            materialMap.Add(
                abilityCrest.UniqueBuildupMaterialId,
                levelInfo.UniqueBuildupMaterialCount
            );
        }

        if (!await ValidateCost(materialMap))
        {
            return ResultCode.AbilityCrestBuildupPieceStepError;
        }

        DbAbilityCrest dbAbilityCrest = await TryFindAsync(abilityCrest.Id);

        if (!ValidateStep(dbAbilityCrest.BuildupCount, buildup.step))
        {
            return ResultCode.AbilityCrestBuildupPieceStepError;
        }

        AbilityCrestRarity rarityInfo = MasterAsset.AbilityCrestRarity.Get(abilityCrest.Rarity);

        if (!ValidateLevel(rarityInfo, dbAbilityCrest.LimitBreakCount, buildup.step))
        {
            return ResultCode.AbilityCrestBuildupPieceShortLimitBreakCount;
        }

        dbAbilityCrest.BuildupCount = buildup.step;
        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());

        return ResultCode.Success;
    }

    public async Task<ResultCode> TryBuildupAugments(
        AbilityCrest abilityCrest,
        AtgenPlusCountParamsList buildup
    )
    {
        AbilityCrestRarity rarityInfo = MasterAsset.AbilityCrestRarity.Get(abilityCrest.Rarity);

        if (!ValidateAugments(rarityInfo, buildup.plus_count_type, buildup.plus_count))
        {
            return ResultCode.AbilityCrestBuildupPlusCountCountError;
        }

        DbAbilityCrest dbAbilityCrest = await TryFindAsync(abilityCrest.Id);
        int usedAugments;
        Dictionary<Materials, int> materialMap;

        if (buildup.plus_count_type == 1)
        {
            usedAugments = buildup.plus_count - dbAbilityCrest.HpPlusCount;
            materialMap = new() { { Materials.FortifyingGemstone, usedAugments } };
        }
        else
        {
            usedAugments = buildup.plus_count - dbAbilityCrest.AttackPlusCount;
            materialMap = new() { { Materials.AmplifyingGemstone, usedAugments } };
        }

        if (usedAugments < 0 || !await ValidateCost(materialMap))
        {
            return ResultCode.AbilityCrestBuildupPlusCountCountError;
        }

        if (buildup.plus_count_type == 1)
        {
            dbAbilityCrest.HpPlusCount = buildup.plus_count;
        }
        else
        {
            dbAbilityCrest.AttackPlusCount = buildup.plus_count;
        }

        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());
        return ResultCode.Success;
    }

    public async Task<ResultCode> TryResetAugments(AbilityCrests abilityCrestId, int augmentType)
    {
        DbAbilityCrest dbAbilityCrest = await TryFindAsync(abilityCrestId);

        int augmentTotal = augmentType switch
        {
            1 => dbAbilityCrest.HpPlusCount,
            2 => dbAbilityCrest.AttackPlusCount,
            _
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid augment type"
                )
        };

        if (!await ValidateCoinCost(augmentTotal * 20_000))
        {
            return ResultCode.CommonMaterialShort;
        }

        if (augmentType == 1)
        {
            dbAbilityCrest.HpPlusCount = 0;
        }
        else
        {
            dbAbilityCrest.AttackPlusCount = 0;
        }

        await this.userDataRepository.UpdateCoin(-augmentTotal * 20_000);
        return ResultCode.Success;
    }

    private void SetMaterialMapSpecial(
        int rarity,
        AtgenBuildupAbilityCrestPieceList buildup,
        ref Dictionary<Materials, int> materialMap,
        ref int dewpoint
    )
    {
        if (!CheckDedicated(buildup))
        {
            return;
        }

        switch (rarity)
        {
            case 4:
            {
                materialMap = new Dictionary<Materials, int>() { { Materials.SilverKey, 1 } };
                dewpoint = 0;
                return;
            }
            case 5:
            {
                materialMap = new Dictionary<Materials, int>() { { Materials.GoldenKey, 1 } };
                dewpoint = 0;
                return;
            }
            default:
            {
                throw new DragaliaException(
                    ResultCode.AbilityCrestBuildupPieceStepError,
                    $"Cannot use dedicated material to unbind ability crest of rarity {rarity}"
                );
            }
        }
    }

    // if buildup doesn't use dedicated material returns false, if it does and is an unbind then returns true
    // otherwise throws an exception
    private static bool CheckDedicated(AtgenBuildupAbilityCrestPieceList buildup)
    {
        if (!buildup.is_use_dedicated_material)
        {
            return false;
        }

        if (!(buildup.buildup_piece_type == BuildupPieceTypes.Unbind))
        {
            throw new DragaliaException(
                ResultCode.AbilityCrestBuildupPieceStepError,
                "IsUseDedicatedMaterial marked true on invalid buildup type"
            );
        }

        return true;
    }

    private async Task<DbAbilityCrest> TryFindAsync(AbilityCrests abilityCrestId)
    {
        DbAbilityCrest? dbAbilityCrest = await this.abilityCrestRepository.FindAsync(
            abilityCrestId
        );

        if (dbAbilityCrest is null)
        {
            throw new DragaliaException(
                ResultCode.AbilityCrestBuildupPieceUnablePiece,
                $"Player does not own ability crest with id {abilityCrestId}"
            );
        }

        return dbAbilityCrest;
    }

    private async Task<bool> ValidateCost(Dictionary<Materials, int> materialMap)
    {
        if (!await this.inventoryRepository.CheckQuantity(materialMap))
        {
            this.logger.LogWarning("Player doesn't have enough materials to perform action");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateCost(int dewpoint)
    {
        if (!await this.userDataRepository.CheckDewpoint(dewpoint))
        {
            this.logger.LogWarning("Player doesn't have enough dewpoint to perform action");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateCoinCost(int coin)
    {
        if (!await this.userDataRepository.CheckCoin(coin))
        {
            this.logger.LogWarning("Player doesn't have enough coin to perform action");
            return false;
        }

        return true;
    }

    private bool ValidateStep(int currLevel, int step)
    {
        if (step != currLevel + 1)
        {
            this.logger.LogWarning("Cannot upgrade in increments greater than 1");
            return false;
        }

        return true;
    }

    private bool ValidateLevel(AbilityCrestRarity rarityInfo, int limitBreak, int step)
    {
        int levelLimit = limitBreak switch
        {
            0 => rarityInfo.MaxLimitLevelByLimitBreak0,
            1 => rarityInfo.MaxLimitLevelByLimitBreak1,
            2 => rarityInfo.MaxLimitLevelByLimitBreak2,
            3 => rarityInfo.MaxLimitLevelByLimitBreak3,
            4 => rarityInfo.MaxLimitLevelByLimitBreak4,
            _
                => throw new DragaliaException(
                    ResultCode.AbilityCrestBuildupPieceUnablePiece,
                    "Limit break count invalid"
                )
        };

        return step <= levelLimit;
    }

    private bool ValidateAugments(AbilityCrestRarity rarityInfo, int augmentType, int amount)
    {
        int augmentLimit = augmentType switch
        {
            1 => rarityInfo.MaxHpPlusCount,
            2 => rarityInfo.MaxAtkPlusCount,
            _
                => throw new DragaliaException(
                    ResultCode.AbilityCrestBuildupPieceStepError,
                    "Invalid augment type"
                )
        };

        return amount <= augmentLimit;
    }
}
