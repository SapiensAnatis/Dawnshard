using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V14Update(
    IStoryRepository storyRepository,
    IEmblemRepository emblemRepository,
    ILogger<V14Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 14;

    public async Task Apply()
    {
        int[] readStoryIdList = await storyRepository
            .UnitStories
            .Where(
                x =>
                    (x.State == StoryState.Read)
                    && x.StoryType == StoryTypes.Chara
                    && x.StoryId % 10 == 5
            )
            .Select(x => x.StoryId)
            .ToArrayAsync();
        int storyCharacterId;
        int[] characterStoryList;

        HashSet<Emblems> ownedEmblems =
            new(await emblemRepository.Emblems.Select(x => x.EmblemId).ToListAsync());

        foreach (int readStoryId in readStoryIdList)
        {
            storyCharacterId = MasterAsset.UnitStory[readStoryId].ReleaseTriggerId;
            characterStoryList = MasterAsset.CharaStories[storyCharacterId].storyIds;

            Emblems emblem = (Emblems)storyCharacterId;
            if (readStoryId == characterStoryList.Last() && !ownedEmblems.Contains(emblem))
            {
                if (!Enum.IsDefined(emblem))
                    continue;

                logger.LogDebug("Granting emblem {emblem}", emblem);

                emblemRepository.AddEmblem(emblem);
            }
        }
    }
}
