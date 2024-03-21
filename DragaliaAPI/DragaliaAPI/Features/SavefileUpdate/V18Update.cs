using System.Diagnostics;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.SavefileUpdate;

[UsedImplicitly]
public class V18Update(
    IWallService wallService,
    IStoryRepository storyRepository,
    IMissionRepository missionRepository,
    IMissionService missionService,
    ILogger<V18Update> logger
) : ISavefileUpdate
{
    private const int ClearAnyMgMissionId = 10010101; // Clear The Mercurial Gauntlet

    private readonly IWallService wallService = wallService;
    private readonly IStoryRepository storyRepository = storyRepository;
    private readonly IMissionRepository missionRepository = missionRepository;
    private readonly IMissionService missionService = missionService;
    private readonly ILogger<V18Update> logger = logger;

    public int SavefileVersion => 18;

    public async Task Apply()
    {
        if (
            !await this.storyRepository.HasReadQuestStory(
                TutorialService.TutorialStoryIds.MercurialGauntlet
            )
        )
        {
            this.logger.LogInformation("Player has not unlocked wall yet, skipping...");
            return;
        }

        if (
            await this.missionRepository.FindMissionByIdAsync(
                MissionType.Normal,
                ClearAnyMgMissionId
            ) != null
        )
        {
            this.logger.LogInformation("Player has already unlocked wall missions, skipping...");
            return;
        }

        this.logger.LogInformation("Initializing wall missions");

        await this.InitializePreCompletedWallMissions();
    }

    private async Task InitializePreCompletedWallMissions()
    {
        Dictionary<QuestWallTypes, int> wallMap = await this.wallService.GetWallLevelMap();

        await this.missionService.StartMission(MissionType.Normal, ClearAnyMgMissionId);

        foreach ((QuestWallTypes type, int obtainedLevel) in wallMap)
        {
            int currentLevel;
            for (currentLevel = 1; currentLevel <= obtainedLevel; currentLevel++)
            {
                int missionId = GetElementalWallMissionId(type, currentLevel);
                this.missionService.AddCompletedMission(MissionType.Normal, missionId);
            }

            if (currentLevel <= WallService.MaximumQuestWallLevel)
            {
                int currentMissionId = GetElementalWallMissionId(type, currentLevel);
                await this.missionService.StartMission(MissionType.Normal, currentMissionId);
            }
        }

        int minLevel = wallMap.Values.Min();

        int currentMinLevel;
        for (currentMinLevel = 2; currentMinLevel <= minLevel; currentMinLevel += 2)
        {
            int missionId = GetMinLevelWallMissionId(currentMinLevel);
            this.missionService.AddCompletedMission(MissionType.Normal, missionId);
        }

        if (currentMinLevel <= WallService.MaximumQuestWallLevel)
        {
            int currentMinLevelMissionId = GetMinLevelWallMissionId(currentMinLevel);
            await this.missionService.StartMission(MissionType.Normal, currentMinLevelMissionId);
        }
    }

    private static int GetElementalWallMissionId(QuestWallTypes type, int level)
    {
        // Example mission ID: 10010201
        // "Clear The Mercurial Gauntlet (Flame): Lv. 1"
        // Another example: 10010310
        // "Clear The Mercurial Gauntlet (Water): Lv. 10"

        const int baseMissionId = 10010200;
        int elementOffset = type - QuestWallTypes.Flame;

        int missionId = baseMissionId + (elementOffset * 100) + level;
        Debug.Assert(MasterAsset.NormalMission.ContainsKey(missionId));

        return missionId;
    }

    private static int GetMinLevelWallMissionId(int level)
    {
        if (level % 2 != 0)
            throw new ArgumentException("Attempted to get odd-lvl minimum wall mission");

        const int baseMissionId = 10010700;

        int missionId = baseMissionId + (level / 2);
        Debug.Assert(MasterAsset.NormalMission.ContainsKey(missionId));

        return missionId;
    }
}
