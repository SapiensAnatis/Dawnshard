using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Update to fix an issue where Harle and Origa were not correctly unlocked from the event compendium.
/// </summary>
[UsedImplicitly]
public partial class V20Update(
    ApiContext apiContext,
    IPresentService presentService,
    ILogger<V20Update> logger
) : ISavefileUpdate
{
    private const int HarleStoryId = 2044303;
    private const int OrigaStoryId = 2046203;

    public int SavefileVersion => 20;

    public async Task Apply()
    {
        List<int> completedStoryIds = await apiContext
            .PlayerStoryState.Where(x =>
                x.StoryType == StoryTypes.Quest && x.State == StoryState.Read
            )
            .Where(x => x.StoryId == HarleStoryId || x.StoryId == OrigaStoryId)
            .Select(x => x.StoryId)
            .ToListAsync();

        List<Charas> ownedCharaIds = await apiContext
            .PlayerCharaData.Where(x => x.CharaId == Charas.Harle || x.CharaId == Charas.Origa)
            .Select(x => x.CharaId)
            .ToListAsync();

        Log.CompletedStoryIDs(logger, completedStoryIds);
        Log.OwnedCharaIDs(logger, ownedCharaIds);

        if (completedStoryIds.Contains(HarleStoryId) && !ownedCharaIds.Contains(Charas.Harle))
        {
            Log.DetectedHarleAsMissingAddingPresent(logger);

            presentService.AddPresent(
                new Present.Present(
                    PresentMessage.DragaliaLostTeamGift,
                    EntityTypes.Chara,
                    (int)Charas.Harle
                )
            );
        }

        if (completedStoryIds.Contains(OrigaStoryId) && !ownedCharaIds.Contains(Charas.Origa))
        {
            Log.DetectedOrigaAsMissingAddingPresent(logger);

            presentService.AddPresent(
                new Present.Present(
                    PresentMessage.DragaliaLostTeamGift,
                    EntityTypes.Chara,
                    (int)Charas.Origa
                )
            );
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Completed story IDs: {CompletedStoryIds}")]
        public static partial void CompletedStoryIDs(ILogger logger, List<int> completedStoryIds);
        [LoggerMessage(LogLevel.Debug, "Owned chara IDs: {OwnedCharaIds}")]
        public static partial void OwnedCharaIDs(ILogger logger, List<Charas> ownedCharaIds);
        [LoggerMessage(LogLevel.Information, "Detected Harle as missing. Adding present.")]
        public static partial void DetectedHarleAsMissingAddingPresent(ILogger logger);
        [LoggerMessage(LogLevel.Information, "Detected Origa as missing. Adding present.")]
        public static partial void DetectedOrigaAsMissingAddingPresent(ILogger logger);
    }
}
