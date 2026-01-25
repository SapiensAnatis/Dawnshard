using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Grants level up wyrmite for the player's currently reached level, since that was not done previously.
/// </summary>
/// <remarks>
/// This will also run every time a save is imported, but that's not a big problem.
/// </remarks>
public class V26Update(ApiContext apiContext, IPresentService presentService) : ISavefileUpdate
{
    private const int WyrmiteLevelUpReward = 50;

    public int SavefileVersion => 26;

    public async Task Apply()
    {
        int currentLevel = await apiContext.PlayerUserData.Select(x => x.Level).FirstAsync();
        int levelsAdvanced = currentLevel - 1; // Everyone starts at level 1
        int giftQuantity = WyrmiteLevelUpReward * levelsAdvanced;

        presentService.AddPresent(
            new Present.Present(
                MessageId: PresentMessage.PlayerLevelUp,
                EntityType: EntityTypes.Wyrmite,
                EntityId: 0,
                EntityQuantity: giftQuantity
            )
            {
                MessageParamValues = [currentLevel],
            }
        );
    }
}
