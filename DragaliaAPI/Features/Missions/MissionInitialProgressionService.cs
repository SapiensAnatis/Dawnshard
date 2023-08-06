using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionInitialProgressionService(
    ILogger<MissionInitialProgressionService> logger,
    IFortMissionProgressionService fortMissionProgressionService,
    IAbilityCrestRepository abilityCrestRepository,
    IQuestRepository questRepository,
    IUnitRepository unitRepository,
    IWeaponRepository weaponRepository,
    IStoryRepository storyRepository,
    IUserDataRepository userDataRepository
) : IMissionInitialProgressionService
{
    public async Task GetInitialMissionProgress(DbPlayerMission mission)
    {
        Mission missionInfo = Mission.From(mission.Type, mission.Id);

        MissionProgressionInfo? progressionInfo =
            MasterAsset.MissionProgressionInfo.Enumerable.SingleOrDefault(
                x => x.MissionType == mission.Type && x.MissionId == mission.Id
            );

        if (progressionInfo == null)
            return;

        int amountToComplete = missionInfo.CompleteValue;
        int currentAmount = progressionInfo.CompleteType switch
        {
            MissionCompleteType.FortPlantLevelUp
                => await fortMissionProgressionService.GetMaxFortPlantLevel(
                    (FortPlants)progressionInfo.Parameter!
                ),
            MissionCompleteType.FortPlantBuilt
            or MissionCompleteType.FortPlantPlaced
                => await fortMissionProgressionService.GetFortPlantCount(
                    (FortPlants)progressionInfo.Parameter!
                ),
            MissionCompleteType.FortLevelUp
                => await fortMissionProgressionService.GetTotalFortLevel(),
            MissionCompleteType.QuestCleared => await GetQuestClearedCount(progressionInfo),
            MissionCompleteType.QuestStoryCleared
                => await storyRepository.QuestStories
                    .Where(
                        x => x.StoryId == progressionInfo.Parameter && x.State == StoryState.Read
                    )
                    .AnyAsync()
                    ? 1
                    : 0,
            MissionCompleteType.QuestGroupCleared
                => await GetQuestGroupClearedCount(progressionInfo),
            MissionCompleteType.WeaponEarned => await GetWeaponEarnedCount(progressionInfo),
            MissionCompleteType.WeaponRefined => await GetWeaponRefinedCount(progressionInfo),
            MissionCompleteType.AbilityCrestBuildupPlusCount
                => await GetWyrmprintBuildupCount(progressionInfo),
            MissionCompleteType.CharacterBuildupPlusCount
                => await GetCharacterBuildupCount(progressionInfo),
            MissionCompleteType.ItemSummon => 0, // TODO: As daily quests also use this, should we make it actually count the item summons?
            MissionCompleteType.PlayerLevelUp
                => (await userDataRepository.GetUserDataAsync()).Level,
            MissionCompleteType.AccountLinked => 0,
            _
                => throw new UnreachableException(
                    $"Invalid MissionProgressType {progressionInfo.CompleteType} in initial progress handling"
                )
        };

        if (currentAmount == 0)
            return;

        if (currentAmount >= amountToComplete)
        {
            mission.Progress = amountToComplete;
            mission.State = MissionState.Completed;
            logger.LogDebug(
                "Mission {missionId} ({missionType}) had all requirements met ({currentProgress}/{requiredProgress}), auto-completing.",
                mission.Id,
                mission.Type,
                currentAmount,
                amountToComplete
            );
        }
        else
        {
            mission.Progress = currentAmount;
            logger.LogDebug(
                "Mission {missionId} ({missionType}) had some requirements met ({currentProgress}/{requiredProgress}).",
                mission.Id,
                mission.Type,
                currentAmount,
                amountToComplete
            );
        }
    }

    private async Task<int> GetQuestClearedCount(MissionProgressionInfo requirement)
    {
        return await questRepository.Quests
            .Where(x => x.QuestId == requirement.Parameter)
            .Select(x => x.PlayCount)
            .FirstOrDefaultAsync();
    }

    private async Task<int> GetCharacterBuildupCount(MissionProgressionInfo requirement)
    {
        return (PlusCountType)requirement.Parameter! switch
        {
            PlusCountType.Hp => await unitRepository.Charas.Select(x => x.HpPlusCount).MaxAsync(),
            PlusCountType.Atk
                => await unitRepository.Charas.Select(x => x.AttackPlusCount).MaxAsync(),
            _
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    $"Invalid PlusCountType for character in mission requirement, parameter: {requirement.Parameter}"
                ),
        };
    }

    private async Task<int> GetWyrmprintBuildupCount(MissionProgressionInfo requirement)
    {
        return (PlusCountType)requirement.Parameter! switch
        {
            PlusCountType.Hp
                => await abilityCrestRepository.AbilityCrests.Select(x => x.HpPlusCount).MaxAsync(),
            PlusCountType.Atk
                => await abilityCrestRepository.AbilityCrests
                    .Select(x => x.AttackPlusCount)
                    .MaxAsync(),
            _
                => throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    $"Invalid PlusCountType for wyrmprint in mission requirement, parameter: {requirement.Parameter}"
                ),
        };
    }

    private async Task<int> GetWeaponEarnedCount(MissionProgressionInfo requirement)
    {
        List<WeaponBodies> validWeaponBodies = MasterAsset.WeaponBody.Enumerable
            .Where(
                x =>
                    (
                        requirement.Parameter is null
                        || x.ElementalType == (UnitElement)requirement.Parameter
                    )
                    && (requirement.Parameter2 is null || x.Rarity == requirement.Parameter2)
                    && (
                        requirement.Parameter3 is null
                        || x.WeaponSeriesId == (WeaponSeries)requirement.Parameter3
                    )
            )
            .Select(x => x.Id)
            .ToList();

        return await weaponRepository.WeaponBodies.CountAsync(
            x => validWeaponBodies.Contains(x.WeaponBodyId)
        );
    }

    private async Task<int> GetWeaponRefinedCount(MissionProgressionInfo requirement)
    {
        List<WeaponBodies> validWeaponBodies = MasterAsset.WeaponBody.Enumerable
            .Where(
                x =>
                    (
                        requirement.Parameter is null
                        || x.ElementalType == (UnitElement)requirement.Parameter
                    )
                    && (requirement.Parameter2 is null || x.Rarity == requirement.Parameter2)
                    && (
                        requirement.Parameter3 is null
                        || x.WeaponSeriesId == (WeaponSeries)requirement.Parameter3
                    )
            )
            .Select(x => x.Id)
            .ToList();

        return await weaponRepository.WeaponBodies
            .Where(x => validWeaponBodies.Contains(x.WeaponBodyId))
            .SumAsync(x => x.LimitOverCount);
    }

    private async Task<int> GetQuestGroupClearedCount(MissionProgressionInfo requirement)
    {
        List<int> validQuests = MasterAsset.QuestData.Enumerable
            .Where(
                x =>
                    x.Gid == requirement.Parameter
                    || (
                        MasterAsset.QuestEventGroup.TryGetValue(
                            x.Gid,
                            out QuestEventGroup? eventGroup
                        )
                        && eventGroup.BaseQuestGroupId == requirement.Parameter
                    )
            )
            .Select(x => x.Id)
            .ToList();

        return await questRepository.Quests
            .Where(x => validQuests.Contains(x.QuestId))
            .SumAsync(x => x.PlayCount);
    }
}
