using DragaliaAPI.Database;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Summoning;

using CharaNewCheckResult = (Charas Id, bool IsNew);
using DragonNewCheckResult = (Dragons Id, bool IsNew);

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
        IDictionary<int, Entity> inputRewardDict = idList
            .Select((x, index) => KeyValuePair.Create(index, new Entity(EntityTypes.Chara, (int)x)))
            .ToDictionary();

        IDictionary<int, RewardGrantResult> outputRewardDict =
            await rewardService.BatchGrantRewards(inputRewardDict);

        List<CharaNewCheckResult> result = [];

        foreach ((int key, RewardGrantResult grantResult) in outputRewardDict)
        {
            result.Add(((Charas)inputRewardDict[key].Id, grantResult == RewardGrantResult.Added));
        }

        return result;
    }

    public async Task<IList<DragonNewCheckResult>> AddDragons(List<Dragons> idList)
    {
        IDictionary<int, Entity> inputRewardDict = idList
            .Select(
                (x, index) => KeyValuePair.Create(index, new Entity(EntityTypes.Dragon, (int)x))
            )
            .ToDictionary();

        IDictionary<int, RewardGrantResult> outputRewardDict =
            await rewardService.BatchGrantRewards(inputRewardDict);

        IEnumerable<Present.Present> presentsToAdd = outputRewardDict
            .Where(kvp => kvp.Value == RewardGrantResult.Limit)
            .Select(x => new Present.Present(
                PresentMessage.SummonShowcase,
                EntityTypes.Dragon,
                inputRewardDict[x.Key].Id
            ));

        presentService.AddPresent(presentsToAdd);

        List<Dragons> ownedDragons = await apiContext
            .PlayerDragonData.Select(x => x.DragonId)
            .Where(x => idList.Contains(x))
            .ToListAsync();

        List<DragonNewCheckResult> newMapping = MarkNewDragons(ownedDragons, idList);

        return newMapping.Select(x => (x.Id, x.IsNew)).ToList();
    }

    private static List<DragonNewCheckResult> MarkNewDragons(
        List<Dragons> owned,
        List<Dragons> idList
    )
    {
        List<DragonNewCheckResult> result = new();
        foreach (Dragons c in idList)
        {
            bool isDragonNew = !(result.Any(x => x.Id == c) || owned.Contains(c));
            result.Add((c, isDragonNew));
        }

        return result;
    }
}
