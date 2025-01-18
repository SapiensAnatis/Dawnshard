using System.Collections.Frozen;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Shared.Features.StorySkip.StorySkipRewards;

namespace DragaliaAPI.Features.Story.Skip;

public class StorySkipService(
    IMissionProgressionService missionProgressionService,
    FortDataService fortDataService,
    IWallService wallService,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<StorySkipService> logger
)
{
    private static readonly FrozenSet<QuestData> QuestDatas = MasterAsset
        .QuestData.Enumerable.Where(x => x.Gid < 10011 && IsNormalModeQuest(x.Id))
        .ToFrozenSet();

    private static readonly FrozenSet<QuestStory> QuestStories = MasterAsset
        .QuestStory.Enumerable.Where(x => x.GroupId < 10011)
        .ToFrozenSet();

    public async Task IncreaseFortLevels()
    {
        List<DbFortBuild> userForts = await apiContext
            .PlayerFortBuilds.Where(x => FortConfigs.Keys.AsEnumerable().Contains(x.PlantId))
            .ToListAsync();

        int currentFortLevel = await fortDataService.GetTotalFortLevel();

        List<DbFortBuild> newUserForts = [];

        foreach ((FortPlants fortPlant, FortConfig fortConfig) in FortConfigs)
        {
            List<DbFortBuild> fortsToUpdate = userForts.Where(x => x.PlantId == fortPlant).ToList();

            foreach (DbFortBuild fortToUpdate in fortsToUpdate)
            {
                if (fortToUpdate.Level < fortConfig.Level)
                {
                    currentFortLevel += fortConfig.Level - fortToUpdate.Level;

                    logger.LogDebug("Updating fort at BuildId {buildId}", fortToUpdate.BuildId);
                    fortToUpdate.Level = fortConfig.Level;
                    fortToUpdate.BuildStartDate = DateTimeOffset.UnixEpoch;
                    fortToUpdate.BuildEndDate = DateTimeOffset.UnixEpoch;

                    missionProgressionService.EnqueueEvent(
                        MissionCompleteType.FortPlantLevelUp,
                        total: fortConfig.Level,
                        parameter: (int)fortToUpdate.PlantId
                    );

                    missionProgressionService.EnqueueEvent(
                        MissionCompleteType.FortLevelUp,
                        1,
                        currentFortLevel
                    );
                }
            }

            for (int x = fortsToUpdate.Count; x < fortConfig.BuildCount; x++)
            {
                logger.LogDebug("Adding fort {plantId}", fortPlant);
                DbFortBuild newUserFort = new()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    PlantId = fortPlant,
                    Level = fortConfig.Level,
                    PositionX = fortConfig.PositionX,
                    PositionZ = fortConfig.PositionZ,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch,
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch,
                };
                newUserForts.Add(newUserFort);

                currentFortLevel += 1;

                missionProgressionService.EnqueueEvent(
                    MissionCompleteType.FortPlantBuilt,
                    1,
                    fortsToUpdate.Count + 1,
                    (int)fortPlant
                );

                missionProgressionService.EnqueueEvent(
                    MissionCompleteType.FortPlantLevelUp,
                    total: fortConfig.Level,
                    parameter: (int)fortPlant
                );

                missionProgressionService.EnqueueEvent(
                    MissionCompleteType.FortLevelUp,
                    1,
                    currentFortLevel
                );
            }
        }

        if (newUserForts.Count > 0)
        {
            apiContext.PlayerFortBuilds.AddRange(newUserForts);
        }
    }

    public async Task<int> ProcessQuestCompletions()
    {
        int wyrmite = 0;

        List<DbQuest> userQuests = await apiContext.PlayerQuests.ToListAsync();

        List<DbQuest> newUserQuests = [];
        foreach (QuestData questData in QuestDatas)
        {
            bool questExists = userQuests.Any(x => x.QuestId == questData.Id);
            if (questExists == false)
            {
                wyrmite += 25;
                DbQuest userQuest = new()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    QuestId = questData.Id,
                    State = 3,
                    IsMissionClear1 = true,
                    IsMissionClear2 = true,
                    IsMissionClear3 = true,
                    PlayCount = 1,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1,
                    IsAppear = true,
                    BestClearTime = 36000,
                    LastWeeklyResetTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = DateTimeOffset.UnixEpoch,
                };
                newUserQuests.Add(userQuest);

                missionProgressionService.OnQuestCleared(
                    questData.Id,
                    questData.Gid,
                    questData.QuestPlayModeType,
                    1,
                    1
                );
            }
            else
            {
                DbQuest userQuest = userQuests.First(x => x.QuestId == questData.Id);
                bool isFirstClear = userQuest.State < 3;
                if (isFirstClear)
                {
                    wyrmite += 10;

                    missionProgressionService.OnQuestCleared(
                        questData.Id,
                        questData.Gid,
                        questData.QuestPlayModeType,
                        1,
                        1
                    );
                }

                if (!userQuest.IsMissionClear1)
                {
                    userQuest.IsMissionClear1 = true;
                    wyrmite += 5;
                }

                if (!userQuest.IsMissionClear2)
                {
                    userQuest.IsMissionClear2 = true;
                    wyrmite += 5;
                }

                if (!userQuest.IsMissionClear3)
                {
                    userQuest.IsMissionClear3 = true;
                    wyrmite += 5;
                }

                if (userQuest.BestClearTime < 0)
                {
                    userQuest.BestClearTime = 36000;
                }

                userQuest.PlayCount += 1;
                userQuest.DailyPlayCount += 1;
                userQuest.WeeklyPlayCount += 1;
                userQuest.State = 3;
            }
        }

        if (newUserQuests.Count > 0)
        {
            apiContext.PlayerQuests.AddRange(newUserQuests);
        }

        return wyrmite;
    }

    public async Task<int> ProcessStoryCompletions()
    {
        int wyrmite = 0;

        List<DbPlayerStoryState> userStories = await apiContext
            .PlayerStoryState.Where(x => x.StoryType == StoryTypes.Quest)
            .ToListAsync();

        List<DbPlayerStoryState> newUserStories = [];
        foreach (QuestStory questStory in QuestStories)
        {
            DbPlayerStoryState? storyState = userStories.FirstOrDefault(x =>
                x.StoryId == questStory.Id
            );

            if (storyState is null)
            {
                wyrmite += 25;
                DbPlayerStoryState userStory = new()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    StoryType = StoryTypes.Quest,
                    StoryId = questStory.Id,
                    State = StoryState.Read,
                };
                newUserStories.Add(userStory);

                missionProgressionService.OnQuestStoryCleared(questStory.Id);
            }
            else if (storyState.State != StoryState.Read)
            {
                storyState.State = StoryState.Read;

                missionProgressionService.OnQuestStoryCleared(questStory.Id);
            }
        }

        if (newUserStories.Count > 0)
        {
            apiContext.PlayerStoryState.AddRange(newUserStories);
        }

        return wyrmite;
    }

    public Task InitializeWall() => wallService.InitializeWall();

    public async Task RewardCharas()
    {
        List<Charas> userCharas = await apiContext
            .PlayerCharaData.Select(x => x.CharaId)
            .Where(x => CharasList.AsEnumerable().Contains(x))
            .ToListAsync();

        foreach (Charas chara in CharasList)
        {
            bool charaExists = userCharas.Contains(chara);

            if (!charaExists)
            {
                logger.LogDebug("Rewarding character {chara}", chara);
                CharaData charaData = MasterAsset.CharaData[chara];
                StoryData storyData = MasterAsset.CharaStories[(int)chara]; // Every character we add here has stories

                DbPlayerCharaData newUserChara = new()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    CharaId = chara,
                    Rarity = 4,
                    Exp = 0,
                    Level = 1,
                    HpPlusCount = 0,
                    AttackPlusCount = 0,
                    IsNew = true,
                    Skill1Level = 1,
                    Skill2Level = 0,
                    Ability1Level = 1,
                    Ability2Level = 0,
                    Ability3Level = 0,
                    BurstAttackLevel = 0,
                    ComboBuildupCount = 0,
                    HpBase = (ushort)charaData.MinHp4,
                    HpNode = 0,
                    AttackBase = (ushort)charaData.MinAtk4,
                    AttackNode = 0,
                    ExAbilityLevel = 1,
                    ExAbility2Level = 1,
                    IsTemporary = false,
                };
                DbPlayerStoryState newCharaStory = new DbPlayerStoryState()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = storyData.StoryIds[0],
                };

                apiContext.PlayerCharaData.Add(newUserChara);
                apiContext.PlayerStoryState.Add(newCharaStory);
            }
        }
    }

    public async Task RewardDragons()
    {
        List<DragonId> userDragonReliabilities = await apiContext
            .PlayerDragonReliability.Select(x => x.DragonId)
            .Where(x => DragonList.AsEnumerable().Contains(x))
            .ToListAsync();

        foreach (DragonId dragon in DragonList)
        {
            // We check the reliability as it's possible the user had the dragon before and sold it.
            // If they don't have the reliability, we know it's safe to add both entities.
            bool dragonExists = userDragonReliabilities.Contains(dragon);

            if (!dragonExists)
            {
                logger.LogDebug("Rewarding dragon {dragon}", dragon);

                DbPlayerDragonData newDragonEntity = new()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    DragonId = dragon,
                    Exp = 0,
                    Level = 1,
                    HpPlusCount = 0,
                    AttackPlusCount = 0,
                    LimitBreakCount = 0,
                    IsLock = false,
                    IsNew = true,
                    Skill1Level = 1,
                    Ability1Level = 1,
                    Ability2Level = 1,
                };

                DbPlayerDragonReliability newReliability = new(
                    playerIdentityService.ViewerId,
                    dragon
                );

                apiContext.PlayerDragonData.Add(newDragonEntity);
                apiContext.PlayerDragonReliability.Add(newReliability);
            }
        }
    }

    public async Task UpdateUserData(int wyrmiteToAdd)
    {
        DbPlayerUserData data = await apiContext.PlayerUserData.FirstAsync();

        const int maxLevel = 60;
        const int maxExp = 69990;

        data.TutorialFlag = 16640603;
        data.TutorialStatus = 60999;
        data.StaminaSingle = 999;
        data.StaminaMulti = 99;
        data.Crystal += wyrmiteToAdd;

        if (data.Exp < maxExp)
        {
            data.Exp = maxExp;
        }

        if (data.Level < maxLevel)
        {
            data.Level = maxLevel;
        }
    }

    private static bool IsNormalModeQuest(int questId)
    {
        // Select quest with a subgroup like 100010|107| instead of 100010|207| (the latter is hard mode)
        int subgroup = questId % 1000;
        return subgroup is > 100 and < 200;
    }
}
