using System.Diagnostics;
using System.Runtime.CompilerServices;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionInitialProgressionService : IMissionInitialProgressionService
{
    private readonly ILogger<MissionInitialProgressionService> logger;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IFortRepository fortRepository;
    private readonly IQuestRepository questRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IWeaponRepository weaponRepository;
    private readonly IStoryRepository storyRepository;

    public MissionInitialProgressionService(
        ILogger<MissionInitialProgressionService> logger,
        IAbilityCrestRepository abilityCrestRepository,
        IFortRepository fortRepository,
        IQuestRepository questRepository,
        IUnitRepository unitRepository,
        IWeaponRepository weaponRepository,
        IStoryRepository storyRepository
    )
    {
        this.logger = logger;
        this.abilityCrestRepository = abilityCrestRepository;
        this.fortRepository = fortRepository;
        this.questRepository = questRepository;
        this.unitRepository = unitRepository;
        this.weaponRepository = weaponRepository;
        this.storyRepository = storyRepository;
    }

    public async Task GetInitialMissionProgress(DbPlayerMission mission)
    {
        Mission missionInfo = Mission.From(mission.Type, mission.Id);

        MissionInfo info = new(mission.Id, mission.Type);
        MissionProgressionInfo? progressionInfo =
            MasterAsset.MissionProgressionInfo.Enumerable.FirstOrDefault(
                x => x.Requirements.Any(x => x.Missions.Contains(info))
            );

        if (progressionInfo == null)
            return;

        MissionProgressionRequirement requirement = progressionInfo.Requirements.First(
            x => x.Missions.Contains(info)
        );

        int amountToComplete = missionInfo.CompleteValue;
        int currentAmount = progressionInfo.Type switch
        {
            MissionProgressType.FortPlantBuilt
                => await this.fortRepository.Builds.CountAsync(
                    x => x.PlantId == (FortPlants)requirement.Parameter!
                ),
            MissionProgressType.FortPlantUpgraded
                => await this.fortRepository.Builds
                    .Where(x => x.PlantId == (FortPlants)requirement.Parameter!)
                    .Select(x => x.Level)
                    .FirstOrDefaultAsync(),
            MissionProgressType.FortLevelup
                => await this.fortRepository.Builds
                    .Where(x => x.PlantId != FortPlants.TheHalidom)
                    .SumAsync(
                        x => x.BuildEndDate == DateTimeOffset.UnixEpoch ? x.Level : x.Level - 1
                    ),
            MissionProgressType.QuestCleared => await GetQuestClearedCount(requirement),
            MissionProgressType.CharacterBuildup => await GetCharacterBuildupCount(requirement),
            MissionProgressType.WyrmprintAugmentBuildup
                => await GetWyrmprintBuildupCount(requirement),
            MissionProgressType.WeaponEarned => await GetWeaponEarnedCount(requirement),
            MissionProgressType.WeaponRefined => await GetWeaponRefinedCount(requirement),
            MissionProgressType.VoidBattleCleared => await GetVoidBattleClearedCount(requirement),
            _
                => throw new UnreachableException(
                    $"Invalid MissionProgressType {progressionInfo.Type} in initial progress handling"
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

    private async Task<int> GetQuestClearedCount(MissionProgressionRequirement requirement)
    {
        int questClearCount = await this.questRepository.Quests
            .Where(x => x.QuestId == requirement.Parameter)
            .Select(x => x.PlayCount)
            .FirstOrDefaultAsync();

        if (questClearCount == 0)
        {
            // Okay, this might be a quest story.
            if (
                await this.storyRepository.QuestStories
                    .Where(x => x.StoryId == requirement.Parameter && x.State == StoryState.Read)
                    .AnyAsync()
            )
            {
                return 1;
            }
        }

        return questClearCount;
    }

    private async Task<int> GetCharacterBuildupCount(MissionProgressionRequirement requirement)
    {
        if ((PlusCountType)requirement.Parameter! == PlusCountType.Hp)
            return await this.unitRepository.Charas.Select(x => x.HpPlusCount).MaxAsync();

        return await this.unitRepository.Charas.Select(x => x.AttackPlusCount).MaxAsync();
    }

    private async Task<int> GetWyrmprintBuildupCount(MissionProgressionRequirement requirement)
    {
        if ((PlusCountType)requirement.Parameter! == PlusCountType.Hp)
            return await this.abilityCrestRepository.AbilityCrests
                .Select(x => x.HpPlusCount)
                .MaxAsync();

        return await this.abilityCrestRepository.AbilityCrests
            .Select(x => x.AttackPlusCount)
            .MaxAsync();
    }

    private async Task<int> GetWeaponEarnedCount(MissionProgressionRequirement requirement)
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

        return await this.weaponRepository.WeaponBodies.CountAsync(
            x => validWeaponBodies.Contains(x.WeaponBodyId)
        );
    }

    private async Task<int> GetWeaponRefinedCount(MissionProgressionRequirement requirement)
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

        return await this.weaponRepository.WeaponBodies
            .Where(x => validWeaponBodies.Contains(x.WeaponBodyId))
            .SumAsync(x => x.LimitOverCount);
    }

    private async Task<int> GetVoidBattleClearedCount(MissionProgressionRequirement requirement)
    {
        List<int> validQuests = MasterAsset.QuestData.Enumerable
            .Where(x => x.IsPartOfVoidBattleGroups)
            .Select(x => x.Id)
            .ToList();

        return await this.questRepository.Quests
            .Where(x => validQuests.Contains(x.QuestId))
            .SumAsync(x => x.PlayCount);
    }
}
