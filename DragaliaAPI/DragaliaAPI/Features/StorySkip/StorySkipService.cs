using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.StorySkip;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;
using static DragaliaAPI.Shared.Features.StorySkip.FortConfigurations;

namespace DragaliaAPI.Features.StorySkip;

public class StorySkipService(
    IFortRepository fortRepository,
    IFortService fortService,
    ILogger<StorySkipService> logger,
    IQuestCompletionService questCompletionService,
    IQuestRepository questRepository,
    IQuestTreasureService questTreasureService,
    IRewardService rewardService,
    IStoryService storyService,
    IUpdateDataService updateDataService,
    IUserDataRepository userDataRepository
) : IStorySkipService
{
    private async Task<QuestMissionStatus> CompleteQuestMissions(int questId, bool[] currentState)
    {
        List<AtgenMissionsClearSet> clearSet = new();

        bool[] newState = { true, true, true };

        QuestRewardData rewardData = MasterAsset.QuestRewardData[questId];
        for (int i = 0; i < 3; i++)
        {
            (QuestCompleteType type, int value) = rewardData.Missions[i];

            if (!currentState[i])
            {
                newState[i] = true;
                (EntityTypes entity, int id, int quantity) = rewardData.Entities[i];
                await rewardService.GrantReward(new Entity(entity, id, quantity));
                logger.LogDebug("Completed quest mission {missionId}", i);
                clearSet.Add(new AtgenMissionsClearSet(id, entity, quantity, i + 1));
            }
        }

        List<AtgenFirstClearSet> completeSet = new();

        if (currentState.Any(x => !x) && newState.All(x => x))
        {
            await rewardService.GrantReward(
                new Entity(
                    rewardData.MissionCompleteEntityType,
                    rewardData.MissionCompleteEntityId,
                    rewardData.MissionCompleteEntityQuantity
                )
            );
            logger.LogDebug("Granting bonus for completing all missions");
            completeSet.Add(
                new AtgenFirstClearSet(
                    rewardData.MissionCompleteEntityId,
                    rewardData.MissionCompleteEntityType,
                    rewardData.MissionCompleteEntityQuantity
                )
            );
        }

        return new QuestMissionStatus(newState, clearSet, completeSet);
    }

    public async Task IncreaseFortLevels()
    {
        Dictionary<FortPlants, FortConfig> fortConfigs = FortConfigurations.FortConfigs;

        foreach (KeyValuePair<FortPlants, FortConfig> keyValuePair in fortConfigs)
        {
            FortPlants fortPlant = keyValuePair.Key;
            FortConfig fortConfig = keyValuePair.Value;
            await fortRepository.AddToStorage(
                fortPlant,
                fortConfig.BuildCount,
                true,
                fortConfig.Level
            );
        }

        IEnumerable<BuildList> buildListEnum = await fortService.GetBuildList();
        foreach (BuildList buildList in buildListEnum)
        {
            if (fortConfigs.TryGetValue(buildList.PlantId, out FortConfig fortConfig))
            {
                DbFortBuild build = await fortRepository.GetBuilding((long)buildList.BuildId);
                if (build.Level < fortConfig.Level)
                {
                    build.Level = fortConfig.Level;
                    build.BuildStartDate = DateTimeOffset.UnixEpoch;
                    build.BuildEndDate = DateTimeOffset.UnixEpoch;
                }
            }
        }
    }

    public async Task IncreasePlayerLevel()
    {
        const int MaxLevel = 60;
        const int MaxExp = 69990;
        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();
        data.TutorialFlag = 16640603;
        data.TutorialStatus = 60999;
        data.StaminaSingle = 999;
        data.StaminaMulti = 99;

        if (data.Exp < MaxExp)
        {
            data.Exp = MaxExp;
        }

        if (data.Level < MaxLevel)
        {
            data.Level = MaxLevel;
        }
    }

    public async Task OpenTreasure(int questTreasureId, CancellationToken cancellationToken)
    {
        IQueryable<DbQuestTreasureList> questTreasureLists = questRepository.QuestTreasureList;
        if (!questTreasureLists.Any(x => x.QuestTreasureId == questTreasureId))
        {
            QuestOpenTreasureRequest req = new() { QuestTreasureId = questTreasureId };
            await questTreasureService.DoOpenTreasure(req, cancellationToken);
        }
    }

    public async Task ProcessQuestCompletion(int questId)
    {
        DbQuest questData = await questRepository.GetQuestDataAsync(questId);

        bool isFirstClear = questData.State < 3;
        if (isFirstClear)
            await questCompletionService.GrantFirstClearRewards(questData.QuestId);

        bool[] oldMissionStatus =
        {
            questData.IsMissionClear1,
            questData.IsMissionClear2,
            questData.IsMissionClear3
        };

        QuestMissionStatus status = await CompleteQuestMissions(
            questId,
            oldMissionStatus
        );

        questData.IsMissionClear1 = status.Missions[0];
        questData.IsMissionClear2 = status.Missions[1];
        questData.IsMissionClear3 = status.Missions[2];

        if (questData.BestClearTime == -1)
        {
            questData.BestClearTime = 36000;
        }

        questData.PlayCount += 1;
        questData.DailyPlayCount += 1;
        questData.WeeklyPlayCount += 1;
        questData.State = 3;
    }

    public async Task ReadStory(int questStoryId, CancellationToken cancellationToken)
    {
        IEnumerable<AtgenBuildEventRewardEntityList> rewardList = await storyService.ReadStory(
            StoryTypes.Quest,
            questStoryId
        );

        EntityResult entityResult = StoryService.GetEntityResult(rewardList);
        IEnumerable<AtgenQuestStoryRewardList> questRewardList = rewardList.Select(
            StoryService.ToQuestStoryReward
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(
            cancellationToken
        );
    }

}