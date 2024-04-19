// using DragaliaAPI.AutoMapper.Profiles;
// using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Quest;

// using DragaliaAPI.Features.Dungeon.Record;
// using DragaliaAPI.Features.Quest;
// using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
// using DragaliaAPI.Features.StorySkip;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
// using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;
// using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.StorySkip;

public class StorySkipService(
    ILogger<StorySkipService> logger,
    IQuestCompletionService questCompletionService,
    IQuestRepository questRepository,
    IRewardService rewardService,
    IStoryService storyService,
    IUpdateDataService updateDataService
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

    public async Task<object> ProcessQuestCompletion(int questId)
    {
        DbQuest questData = await questRepository.GetQuestDataAsync(questId);

        bool isFirstClear = questData.State < 3;

        IEnumerable<AtgenFirstClearSet> firstClearRewards = isFirstClear
            ? await questCompletionService.GrantFirstClearRewards(questData.QuestId)
            : Enumerable.Empty<AtgenFirstClearSet>();

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

        return (status, firstClearRewards);
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