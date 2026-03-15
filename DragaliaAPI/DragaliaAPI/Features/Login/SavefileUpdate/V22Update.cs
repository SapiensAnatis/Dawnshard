using DragaliaAPI.Database;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Update to grant chapter 10 completion rewards to players that previously cleared chapter 10 and did not receive rewards.
/// </summary>
[UsedImplicitly]
public partial class V22Update(
    ApiContext apiContext,
    IPresentService presentService,
    IUserService userService,
    ILogger<V22Update> logger
) : ISavefileUpdate
{
    private const int Chapter10LastStoryId = 1001009;

    private static readonly List<QuestStoryReward> Rewards = MasterAsset
        .QuestStoryRewardInfo[Chapter10LastStoryId]
        .Rewards.ToList();

    public int SavefileVersion => 22;

    public async Task Apply()
    {
        bool playerCompletedChapter10 = await apiContext
            .PlayerStoryState.Where(x =>
                x.StoryType == StoryTypes.Quest
                && x.StoryId == Chapter10LastStoryId
                && x.State == StoryState.Read
            )
            .AnyAsync();

        Log.PlayerCompletedChapter10(logger, playerCompletedChapter10);

        if (playerCompletedChapter10)
        {
            Log.DetectedThatChapter10WasCompletedGrantingCompletionRewards(logger);

            await userService.AddExperience(69990);

            foreach (QuestStoryReward reward in Rewards)
            {
                if (reward.Type is EntityTypes.Material or EntityTypes.HustleHammer)
                {
                    presentService.AddPresent(
                        new Present.Present(
                            PresentMessage.Chapter10Clear,
                            (EntityTypes)reward.Type,
                            reward.Id,
                            reward.Quantity
                        )
                    );
                }
            }
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Player completed chapter 10: {PlayerCompletedChapter10}")]
        public static partial void PlayerCompletedChapter10(ILogger logger, bool playerCompletedChapter10);
        [LoggerMessage(LogLevel.Information, "Detected that chapter 10 was completed. Granting completion rewards.")]
        public static partial void DetectedThatChapter10WasCompletedGrantingCompletionRewards(ILogger logger);
    }
}
