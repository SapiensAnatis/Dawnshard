using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V14Update(
    IStoryRepository storyRepository,
    IEmblemRepository emblemRepository,
    ApiContext apiContext,
    ILogger<V14Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 14;

    public async Task Apply()
    {
        int[] readStoryIdList = await storyRepository
            .UnitStories.Where(x =>
                (x.State == StoryState.Read)
                && x.StoryType == StoryTypes.Chara
                && x.StoryId % 10 == 5
            )
            .Select(x => x.StoryId)
            .ToArrayAsync();
        int storyCharacterId;
        int[] characterStoryList;

        HashSet<Emblems> ownedEmblems = await emblemRepository
            .Emblems.Select(x => x.EmblemId)
            .ToHashSetAsync();

        foreach (int readStoryId in readStoryIdList)
        {
            storyCharacterId = MasterAsset.UnitStory[readStoryId].ReleaseTriggerId;
            characterStoryList = MasterAsset.CharaStories[storyCharacterId].storyIds;

            Emblems emblem = (Emblems)storyCharacterId;

            if (readStoryId != characterStoryList.Last() || ownedEmblems.Contains(emblem))
                continue;

            if (!Enum.IsDefined(emblem))
                continue;

            // Avoid conflicts in case V10Update has added an equipped story epithet to the change tracker
            if (apiContext.ChangeTracker.Entries<DbEmblem>().Any(x => x.Entity.EmblemId == emblem))
                continue;

            logger.LogDebug("Granting emblem {emblem}", emblem);

            emblemRepository.AddEmblem(emblem);
        }
    }
}
