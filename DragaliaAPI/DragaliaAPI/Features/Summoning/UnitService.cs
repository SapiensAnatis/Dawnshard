using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using Microsoft.EntityFrameworkCore;
using CharaNewCheckResult = (DragaliaAPI.Shared.Definitions.Enums.Charas Id, bool IsNew);
using DragonNewCheckResult = (DragaliaAPI.Shared.Definitions.Enums.DragonId Id, bool IsNew);

namespace DragaliaAPI.Features.Summoning;

/// <summary>
/// Service to assist with adding units after a summon result is generated.
/// </summary>
/// <param name="apiContext">Instance of <see cref="ApiContext"/>.</param>
public class UnitService(
    IPresentService presentService,
    IRewardService rewardService,
    ApiContext apiContext
)
{
    /// <summary>
    /// Add a list of characters to the database. Will only add the first instance of any new character.
    /// </summary>
    /// <returns>
    /// A list of tuples which adds a dimension onto the input list, where the second item shows whether the
    /// given character ID was a duplicate.
    /// </returns>
    public async Task<IList<CharaNewCheckResult>> AddCharas(IList<Charas> idList)
    {
        Dictionary<int, Entity> inputRewardDict = idList
            .Select((x, index) => KeyValuePair.Create(index, new Entity(EntityTypes.Chara, (int)x)))
            .ToDictionary();

        List<CharaNewCheckResult> result = [];

        await rewardService.BatchGrantRewards(
            inputRewardDict,
            (_, entity, grantResult) =>
            {
                result.Add(((Charas)entity.Id, grantResult == RewardGrantResult.Added));
            }
        );

        return result;
    }

    public async Task<IList<DragonNewCheckResult>> AddDragons(List<DragonId> idList)
    {
        Dictionary<int, Entity> inputRewardDict = idList
            .Select(
                (x, index) => KeyValuePair.Create(index, new Entity(EntityTypes.Dragon, (int)x))
            )
            .ToDictionary();

        await rewardService.BatchGrantRewards(
            inputRewardDict,
            (_, entity, grantResult) =>
            {
                if (grantResult == RewardGrantResult.Limit)
                {
                    presentService.AddPresent(
                        new Present.Present(PresentMessage.SummonShowcase, entity.Type, entity.Id)
                    );
                }
            }
        );

        List<DragonId> ownedDragons = await apiContext
            .PlayerDragonData.Select(x => x.DragonId)
            .Where(x => idList.Contains(x))
            .ToListAsync();

        List<DragonNewCheckResult> newMapping = MarkNewDragons(ownedDragons, idList);

        return newMapping.Select(x => (x.Id, x.IsNew)).ToList();
    }

    private static List<DragonNewCheckResult> MarkNewDragons(
        List<DragonId> owned,
        List<DragonId> idList
    )
    {
        List<DragonNewCheckResult> result = new();
        foreach (DragonId c in idList)
        {
            bool isDragonNew = !(result.Any(x => x.Id == c) || owned.Contains(c));
            result.Add((c, isDragonNew));
        }

        return result;
    }
}
