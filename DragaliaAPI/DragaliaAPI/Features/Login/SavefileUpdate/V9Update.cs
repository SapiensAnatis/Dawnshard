using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Fixes missing stories for 3* characters due to issue #358
/// </summary>
public partial class V9Update(
    IStoryRepository storyRepository,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<V9Update> logger
) : ISavefileUpdate
{
    public int SavefileVersion => 9;

    public async Task Apply()
    {
        IEnumerable<(Charas Id, SortedSet<int> ManaNodes)> charaManaData = (
            await apiContext
                .PlayerCharaData
                /* .Where(x => x.Rarity == 3) // A 3-star character could have been upgraded */
                .Where(x => x.CharaId != Charas.ThePrince && x.CharaId != Charas.MegaMan) // No stories
                .Select(x => new { x.CharaId, x.ManaNodeUnlockCount })
                .ToListAsync()
        ).Select(x => new ValueTuple<Charas, SortedSet<int>>(
            x.CharaId,
            ManaNodesUtil.GetSetFromManaNodes((ManaNodes)x.ManaNodeUnlockCount)
        ));

        HashSet<int> stories = (
            await storyRepository.UnitStories.Select(x => x.StoryId).ToListAsync()
        ).ToHashSet();

        foreach ((Charas chara, SortedSet<int> manaNodes) in charaManaData)
        {
            if (
                !MasterAsset.CharaData.TryGetValue(chara, out CharaData? charaData)
                || !MasterAsset.CharaStories.TryGetValue((int)chara, out StoryData? storyData)
            )
            {
                Log.SkippingUnknownCharacter(logger, chara);
                continue;
            }

            int[] storyArray = storyData.StoryIds;

            if (!stories.Contains(storyArray[0]))
            {
                Log.AddingMissingFirstStoryForChara(logger, storyArray[0], chara);
                this.AddStory(storyArray[0]);
            }

            int storyArrayIdx = 0;
            foreach (int nodeNum in manaNodes)
            {
                ManaNode node = charaData.GetManaNode(nodeNum);
                if (!node.IsReleaseStory)
                    continue;

                storyArrayIdx++;

                int storyId;
                try
                {
                    storyId = storyArray[storyArrayIdx];
                }
                catch
                {
                    Log.FailedToLookUpStoryForChara(logger, storyArrayIdx, chara);

                    continue;
                }

                if (stories.Contains(storyId))
                    continue;

                Log.AddingMissingStoryEpisodeForChara(logger, storyId, storyArrayIdx + 1, chara);
                this.AddStory(storyId);
            }
        }
    }

    private void AddStory(int storyId)
    {
        apiContext.PlayerStoryState.Add(
            new DbPlayerStoryState()
            {
                ViewerId = playerIdentityService.ViewerId,
                StoryId = storyId,
                State = 0,
                StoryType = StoryTypes.Chara,
            }
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Skipping unknown character {chara}")]
        public static partial void SkippingUnknownCharacter(ILogger logger, Charas chara);

        [LoggerMessage(LogLevel.Debug, "Adding missing first story {storyId} for chara {chara}")]
        public static partial void AddingMissingFirstStoryForChara(
            ILogger logger,
            int storyId,
            Charas chara
        );

        [LoggerMessage(LogLevel.Error, "Failed to look up story {num} for chara {chara}")]
        public static partial void FailedToLookUpStoryForChara(
            ILogger logger,
            int num,
            Charas chara
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Adding missing story {storyId} (episode {episode}) for chara {chara}"
        )]
        public static partial void AddingMissingStoryEpisodeForChara(
            ILogger logger,
            int storyId,
            int episode,
            Charas chara
        );
    }
}
