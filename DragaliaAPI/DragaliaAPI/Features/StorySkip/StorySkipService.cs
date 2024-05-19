using System.Collections.Frozen;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.StorySkip;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Shared.Features.StorySkip.StorySkipRewards;

namespace DragaliaAPI.Features.StorySkip;

public class StorySkipService(
    ApiContext apiContext,
    IFortRepository fortRepository,
    ILogger<StorySkipService> logger,
    IQuestRepository questRepository,
    IStoryRepository storyRepository,
    IUserDataRepository userDataRepository
)
{
    private static readonly FrozenSet<QuestData> questDatas = MasterAsset
        .QuestData.Enumerable.Where(x =>
            x.Gid < 10011 && x.Id > 100000000 && x.Id.ToString().Substring(6, 1) == "1"
        )
        .ToFrozenSet();

    private static readonly FrozenSet<QuestStory> questStories = MasterAsset
        .QuestStory.Enumerable.Where(x => x.GroupId is < 10011)
        .ToFrozenSet();

    public async Task IncreaseFortLevels(long viewerId)
    {
        Dictionary<FortPlants, FortConfig> fortConfigs = StorySkipRewards.FortConfigs;

        List<DbFortBuild> userForts = await fortRepository
            .Builds.Where(x => x.ViewerId == viewerId)
            .ToListAsync();

        List<DbFortBuild> newUserForts = new();

        foreach ((FortPlants fortPlant, FortConfig fortConfig) in fortConfigs)
        {
            List<DbFortBuild> fortsToUpdate = userForts.Where(x => x.PlantId == fortPlant).ToList();

            foreach (DbFortBuild fortToUpdate in fortsToUpdate)
            {
                if (fortToUpdate.Level < fortConfig.Level)
                {
                    logger.LogDebug("Updating fort at BuildId {buildId}", fortToUpdate.BuildId);
                    fortToUpdate.Level = fortConfig.Level;
                    fortToUpdate.BuildStartDate = DateTimeOffset.UnixEpoch;
                    fortToUpdate.BuildEndDate = DateTimeOffset.UnixEpoch;
                }
            }

            for (int x = fortsToUpdate.Count; x < fortConfig.BuildCount; x++)
            {
                logger.LogDebug("Adding fort {plantId}", fortPlant);
                DbFortBuild newUserFort =
                    new()
                    {
                        ViewerId = viewerId,
                        PlantId = fortPlant,
                        Level = fortConfig.Level,
                        PositionX = fortConfig.PositionX,
                        PositionZ = fortConfig.PositionZ,
                        BuildStartDate = DateTimeOffset.UnixEpoch,
                        BuildEndDate = DateTimeOffset.UnixEpoch,
                        IsNew = true,
                        LastIncomeDate = DateTimeOffset.UnixEpoch
                    };
                newUserForts.Add(newUserFort);
            }
        }

        if (newUserForts.Count > 0)
        {
            apiContext.PlayerFortBuilds.AddRange(newUserForts);
        }
    }

    public async Task<int> ProcessQuestCompletions(long viewerId)
    {
        int wyrmite = 0;

        List<DbQuest> userQuests = await questRepository
            .Quests.Where(x => x.ViewerId == viewerId)
            .ToListAsync();

        List<DbQuest> newUserQuests = new();
        foreach (QuestData questData in questDatas)
        {
            bool questExists = userQuests.Where(x => x.QuestId == questData.Id).Any();
            if (questExists == false)
            {
                wyrmite += 25;
                DbQuest userQuest =
                    new()
                    {
                        ViewerId = viewerId,
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
                        LastDailyResetTime = DateTimeOffset.UnixEpoch
                    };
                newUserQuests.Add(userQuest);
            }
            else
            {
                DbQuest userQuest = userQuests.Where(x => x.QuestId == questData.Id).First();
                bool isFirstClear = userQuest.State < 3;
                if (isFirstClear)
                    wyrmite += 10;
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

                if (userQuest.BestClearTime == -1)
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

    public async Task<int> ProcessStoryCompletions(long viewerId)
    {
        int wyrmite = 0;

        List<DbPlayerStoryState> userStories = await storyRepository
            .QuestStories.Where(x => x.ViewerId == viewerId)
            .ToListAsync();

        List<DbPlayerStoryState> newUserStories = new();
        foreach (QuestStory questStory in questStories)
        {
            bool storyExists = userStories.Where(x => x.StoryId == questStory.Id).Any();
            if (storyExists == false)
            {
                wyrmite += 25;
                DbPlayerStoryState userStory =
                    new()
                    {
                        ViewerId = viewerId,
                        StoryType = StoryTypes.Quest,
                        StoryId = questStory.Id,
                        State = StoryState.Read
                    };
                newUserStories.Add(userStory);
            }
        }

        if (newUserStories.Count > 0)
        {
            apiContext.PlayerStoryState.AddRange(newUserStories);
        }

        return wyrmite;
    }

    public async Task RewardCharas(long viewerId)
    {
        List<Charas> charas = StorySkipRewards.CharasList;
        List<DbPlayerCharaData> userCharas = await apiContext
            .PlayerCharaData.Where(x => x.ViewerId == viewerId && charas.Contains(x.CharaId))
            .ToListAsync();

        List<DbPlayerCharaData> newUserCharas = new();
        foreach (Charas chara in charas)
        {
            bool charaExists = userCharas.Where(x => x.CharaId == chara).Any();
            if (charaExists == false)
            {
                logger.LogDebug("Rewarding character {chara}", chara);
                CharaData charaData = MasterAsset.CharaData[chara];
                DbPlayerCharaData newUserChara =
                    new()
                    {
                        ViewerId = viewerId,
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
                        ListViewFlag = false
                    };
                newUserCharas.Add(newUserChara);
            }
        }

        if (newUserCharas.Count > 0)
        {
            apiContext.PlayerCharaData.AddRange(newUserCharas);
        }
    }

    public async Task RewardDragons(long viewerId)
    {
        List<Dragons> dragons = StorySkipRewards.DragonList;
        List<DbPlayerDragonData> userDragons = await apiContext
            .PlayerDragonData.Where(x => x.ViewerId == viewerId && dragons.Contains(x.DragonId))
            .ToListAsync();

        List<DbPlayerDragonData> newUserDragons = new();
        foreach (Dragons dragon in dragons)
        {
            bool dragonExists = userDragons.Where(x => x.DragonId == dragon).Any();
            if (dragonExists == false)
            {
                logger.LogDebug("Rewarding dragon {dragon}", dragon);
                DbPlayerDragonData newUserDragon =
                    new()
                    {
                        ViewerId = viewerId,
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
                        Ability2Level = 1
                    };
                newUserDragons.Add(newUserDragon);
            }
        }

        if (newUserDragons.Count > 0)
        {
            apiContext.PlayerDragonData.AddRange(newUserDragons);
        }
    }

    public async Task UpdateUserData(int wyrmite)
    {
        const int MaxLevel = 60;
        const int MaxExp = 69990;
        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();
        data.TutorialFlag = 16640603;
        data.TutorialStatus = 60999;
        data.StaminaSingle = 999;
        data.StaminaMulti = 99;
        data.Crystal += wyrmite;

        if (data.Exp < MaxExp)
        {
            data.Exp = MaxExp;
        }

        if (data.Level < MaxLevel)
        {
            data.Level = MaxLevel;
        }
    }
}
