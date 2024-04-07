using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Update to fix an issue where Harle and Origa were not correctly unlocked from the event compendium.
/// </summary>
[UsedImplicitly]
public class V20Update(
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

        logger.LogDebug("Completed story IDs: {CompletedStoryIds}", completedStoryIds);
        logger.LogDebug("Owned chara IDs: {OwnedCharaIds}", ownedCharaIds);

        if (completedStoryIds.Contains(HarleStoryId) && !ownedCharaIds.Contains(Charas.Harle))
        {
            logger.LogInformation("Detected Harle as missing. Adding present.");

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
            logger.LogInformation("Detected Origa as missing. Adding present.");

            presentService.AddPresent(
                new Present.Present(
                    PresentMessage.DragaliaLostTeamGift,
                    EntityTypes.Chara,
                    (int)Charas.Origa
                )
            );
        }
    }
}
