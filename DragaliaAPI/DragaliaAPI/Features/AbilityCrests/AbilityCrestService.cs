using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using static DragaliaAPI.Features.Tutorial.TutorialService.TutorialStatusIds;

namespace DragaliaAPI.Features.AbilityCrests;

public class AbilityCrestService : IAbilityCrestService
{
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IMissionProgressionService missionProgressionService;
    private readonly ITutorialService tutorialService;
    private readonly ILogger<AbilityCrestService> logger;

    public AbilityCrestService(
        IAbilityCrestRepository abilityCrestRepository,
        IInventoryRepository inventoryRepository,
        IUserDataRepository userDataRepository,
        IMissionProgressionService missionProgressionService,
        ITutorialService tutorialService,
        ILogger<AbilityCrestService> logger
    )
    {
        this.abilityCrestRepository = abilityCrestRepository;
        this.inventoryRepository = inventoryRepository;
        this.userDataRepository = userDataRepository;
        this.missionProgressionService = missionProgressionService;
        this.tutorialService = tutorialService;
        this.logger = logger;
    }

    public async Task<ResultCode> TryBuildup(
        AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    )
    {
        ResultCode result = buildup.BuildupPieceType switch
        {
            BuildupPieceTypes.Unbind or BuildupPieceTypes.Copies => await this.TryBuildupGeneric(
                abilityCrest,
                buildup
            ),
            BuildupPieceTypes.Stats => await this.TryBuildupLevel(abilityCrest, buildup),
            _ => throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Invalid buildip piece type"
            ),
        };

        if (result == ResultCode.Success)
        {
            if (buildup.BuildupPieceType == BuildupPieceTypes.Stats)
            {
                await this.tutorialService.AddTutorialFlag(
                    (int)TutorialFlags.AbilityCrestGrowTutorial
                );

                // Go to the unbind tutorial, which is the next step after upgrading stats
                await this.tutorialService.UpdateTutorialStatus(AbilityCrestUnbindTutorial);
            }

            if (buildup.BuildupPieceType == BuildupPieceTypes.Unbind)
            {
                await this.tutorialService.AddTutorialFlag(
                    (int)TutorialFlags.AbilityCrestGrowTutorial
                );

                // Finish the ability crest tutorial
                await this.tutorialService.UpdateTutorialStatus(AbilityCrestTutorialDone);
            }
        }

        return result;
    }

    private async Task<ResultCode> TryBuildupGeneric(
        AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    )
    {
        int buildupId = abilityCrest.GetBuildupGroupId(buildup.BuildupPieceType, buildup.Step);

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

        Dictionary<Materials, int> materialMap = buildupInfo.MaterialMap.ToDictionary();
        int dewpoint = buildupInfo.BuildupDewPoint;

        if (buildupInfo.IsUseUniqueMaterial)
        {
            materialMap[abilityCrest.UniqueBuildupMaterialId] =
                buildupInfo.UniqueBuildupMaterialCount;
        }

        SetMaterialMapSpecial(abilityCrest.Rarity, buildup, ref materialMap, ref dewpoint);

        if (!(await this.ValidateCost(materialMap) && await this.ValidateCost(dewpoint)))
        {
            return ResultCode.AbilityCrestBuildupPieceStepError;
        }

        DbAbilityCrest dbAbilityCrest = await this.TryFindAsync(abilityCrest.Id);

        if (buildup.BuildupPieceType == BuildupPieceTypes.Unbind)
        {
            if (!this.ValidateStep(dbAbilityCrest.LimitBreakCount, buildup.Step))
            {
                return ResultCode.AbilityCrestBuildupPieceStepError;
            }

            dbAbilityCrest.LimitBreakCount = buildup.Step;
        }
        else
        {
            if (!this.ValidateStep(dbAbilityCrest.EquipableCount, buildup.Step))
            {
                return ResultCode.AbilityCrestBuildupPieceStepError;
            }

            dbAbilityCrest.EquipableCount = buildup.Step;
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
        int levelId = abilityCrest.GetBuildupLevelId(buildup.Step);

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
        Dictionary<Materials, int> materialMap = levelInfo.MaterialMap.ToDictionary();

        if (levelInfo.IsUseUniqueMaterial)
        {
            materialMap.Add(
                abilityCrest.UniqueBuildupMaterialId,
                levelInfo.UniqueBuildupMaterialCount
            );
        }

        if (!await this.ValidateCost(materialMap))
        {
            return ResultCode.AbilityCrestBuildupPieceStepError;
        }

        DbAbilityCrest dbAbilityCrest = await this.TryFindAsync(abilityCrest.Id);

        if (!this.ValidateStep(dbAbilityCrest.BuildupCount, buildup.Step))
        {
            return ResultCode.AbilityCrestBuildupPieceStepError;
        }

        AbilityCrestRarity rarityInfo = MasterAsset.AbilityCrestRarity.Get(abilityCrest.Rarity);

        if (!ValidateLevel(rarityInfo, dbAbilityCrest.LimitBreakCount, buildup.Step))
        {
            return ResultCode.AbilityCrestBuildupPieceShortLimitBreakCount;
        }

        this.missionProgressionService.OnAbilityCrestLevelUp(
            dbAbilityCrest.AbilityCrestId,
            buildup.Step - dbAbilityCrest.BuildupCount,
            buildup.Step
        );

        dbAbilityCrest.BuildupCount = buildup.Step;
        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());

        return ResultCode.Success;
    }

    public async Task<ResultCode> TryBuildupAugments(
        AbilityCrest abilityCrest,
        AtgenPlusCountParamsList buildup
    )
    {
        AbilityCrestRarity rarityInfo = MasterAsset.AbilityCrestRarity.Get(abilityCrest.Rarity);

        if (!ValidateAugments(rarityInfo, buildup.PlusCountType, buildup.PlusCount))
        {
            return ResultCode.AbilityCrestBuildupPlusCountCountError;
        }

        DbAbilityCrest dbAbilityCrest = await this.TryFindAsync(abilityCrest.Id);
        int usedAugments;
        Dictionary<Materials, int> materialMap;

        if (buildup.PlusCountType == PlusCountType.Hp)
        {
            usedAugments = buildup.PlusCount - dbAbilityCrest.HpPlusCount;
            materialMap = new() { { Materials.FortifyingGemstone, usedAugments } };
        }
        else
        {
            usedAugments = buildup.PlusCount - dbAbilityCrest.AttackPlusCount;
            materialMap = new() { { Materials.AmplifyingGemstone, usedAugments } };
        }

        if (usedAugments < 0 || !await this.ValidateCost(materialMap))
        {
            return ResultCode.AbilityCrestBuildupPlusCountCountError;
        }

        if (buildup.PlusCountType == PlusCountType.Hp)
        {
            dbAbilityCrest.HpPlusCount = buildup.PlusCount;
        }
        else
        {
            dbAbilityCrest.AttackPlusCount = buildup.PlusCount;
        }

        this.missionProgressionService.OnAbilityCrestBuildupPlusCount(
            dbAbilityCrest.AbilityCrestId,
            buildup.PlusCountType,
            usedAugments,
            buildup.PlusCount
        );

        this.missionProgressionService.OnAbilityCrestTotalPlusCountUp(
            dbAbilityCrest.AbilityCrestId,
            0,
            Math.Min(dbAbilityCrest.HpPlusCount, dbAbilityCrest.AttackPlusCount)
        );

        await this.inventoryRepository.UpdateQuantity(materialMap.Invert());
        return ResultCode.Success;
    }

    public async Task<ResultCode> TryResetAugments(
        AbilityCrestId abilityCrestId,
        PlusCountType augmentType
    )
    {
        DbAbilityCrest dbAbilityCrest = await this.TryFindAsync(abilityCrestId);

        int augmentTotal = augmentType switch
        {
            PlusCountType.Hp => dbAbilityCrest.HpPlusCount,
            PlusCountType.Atk => dbAbilityCrest.AttackPlusCount,
            _ => throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Invalid augment type"
            ),
        };

        if (!await this.userDataRepository.CheckCoin(augmentTotal * 20_000))
        {
            return ResultCode.CommonMaterialShort;
        }

        Dictionary<Materials, int> returnedAugments;
        if (augmentType == PlusCountType.Hp)
        {
            returnedAugments = new()
            {
                { Materials.FortifyingGemstone, dbAbilityCrest.HpPlusCount },
            };
            dbAbilityCrest.HpPlusCount = 0;
        }
        else
        {
            returnedAugments = new()
            {
                { Materials.AmplifyingGemstone, dbAbilityCrest.AttackPlusCount },
            };
            dbAbilityCrest.AttackPlusCount = 0;
        }

        await this.userDataRepository.UpdateCoin(-augmentTotal * 20_000);
        await this.inventoryRepository.UpdateQuantity(returnedAugments);
        return ResultCode.Success;
    }

    private static void SetMaterialMapSpecial(
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
        if (!buildup.IsUseDedicatedMaterial)
        {
            return false;
        }

        if (!(buildup.BuildupPieceType == BuildupPieceTypes.Unbind))
        {
            throw new DragaliaException(
                ResultCode.AbilityCrestBuildupPieceStepError,
                "IsUseDedicatedMaterial marked true on invalid buildup type"
            );
        }

        return true;
    }

    private async Task<DbAbilityCrest> TryFindAsync(AbilityCrestId abilityCrestId)
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

    private bool ValidateStep(int currLevel, int step)
    {
        if (step != currLevel + 1)
        {
            this.logger.LogWarning("Cannot upgrade in increments greater than 1");
            return false;
        }

        return true;
    }

    private static bool ValidateLevel(AbilityCrestRarity rarityInfo, int limitBreak, int step)
    {
        int levelLimit = limitBreak switch
        {
            0 => rarityInfo.MaxLimitLevelByLimitBreak0,
            1 => rarityInfo.MaxLimitLevelByLimitBreak1,
            2 => rarityInfo.MaxLimitLevelByLimitBreak2,
            3 => rarityInfo.MaxLimitLevelByLimitBreak3,
            4 => rarityInfo.MaxLimitLevelByLimitBreak4,
            _ => throw new DragaliaException(
                ResultCode.AbilityCrestBuildupPieceUnablePiece,
                "Limit break count invalid"
            ),
        };

        return step <= levelLimit;
    }

    private static bool ValidateAugments(
        AbilityCrestRarity rarityInfo,
        PlusCountType augmentType,
        int amount
    )
    {
        int augmentLimit = augmentType switch
        {
            PlusCountType.Hp => rarityInfo.MaxHpPlusCount,
            PlusCountType.Atk => rarityInfo.MaxAtkPlusCount,
            _ => throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                "Invalid augment type"
            ),
        };

        return amount <= augmentLimit;
    }
}
