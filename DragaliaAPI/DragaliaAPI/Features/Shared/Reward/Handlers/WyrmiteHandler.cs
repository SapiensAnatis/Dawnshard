using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

public class WyrmiteHandler(ApiContext apiContext) : IRewardHandler, IBatchRewardHandler
{
    // Assumed maximum before UI bugs. True max is int.MaxValue.
    private const int MaxWyrmite = 999_999_999;

    public IReadOnlyList<EntityTypes> SupportedTypes => [EntityTypes.Wyrmite];

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        DbPlayerUserData userData = await apiContext.PlayerUserData.FirstAsync();
        Dictionary<TKey, GrantReturn> results = new(entities.Count);

        foreach ((TKey key, Entity entity) in entities)
        {
            GrantReturn result = TryIncrementCrystal(userData, entity.Quantity)
                ? GrantReturn.Added()
                : GrantReturn.Limit();

            results[key] = result;
        }

        return results;
    }

    public async Task<GrantReturn> Grant(Entity entity)
    {
        DbPlayerUserData userData = await apiContext.PlayerUserData.FirstAsync();

        return TryIncrementCrystal(userData, entity.Quantity)
            ? GrantReturn.Added()
            : GrantReturn.Limit();
    }

    private static bool TryIncrementCrystal(DbPlayerUserData userData, int quantity)
    {
        if (userData.Crystal + quantity > MaxWyrmite)
        {
            return false;
        }

        userData.Crystal += quantity;
        return true;
    }
}
