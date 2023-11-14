using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V14Update(IStoryRepository storyRepository, IRewardService rewardService)
    : ISavefileUpdate
{
    public int SavefileVersion => 14;

    public async Task Apply()
    {
        int[] readStoryIdList = await storyRepository.UnitStories
            .Where(x => (x.State == StoryState.Read) && x.StoryType == StoryTypes.Chara)
            .Select(x => x.StoryId)
            .ToArrayAsync();
        int storyCharacterId;
        int[] characterStoryList;
        foreach (int readStoryId in readStoryIdList)
        {
            storyCharacterId = MasterAsset.UnitStory[readStoryId].ReleaseTriggerId;
            characterStoryList = MasterAsset.CharaStories[storyCharacterId].storyIds;
            if (readStoryId == characterStoryList.Last())
            {
                await rewardService.GrantReward(new(EntityTypes.Title, storyCharacterId, 1));
            }
        }
    }
}
