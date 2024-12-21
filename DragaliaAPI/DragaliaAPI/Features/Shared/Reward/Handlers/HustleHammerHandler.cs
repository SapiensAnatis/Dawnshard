using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

public class HustleHammerHandler(ApiContext apiContext) : IRewardHandler, IBatchRewardHandler
{
    // "Why would you ever need more than this...?"
    private const int MaxHammers = 999_999;

    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.HustleHammer];

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        DbPlayerUserData userData = await apiContext.PlayerUserData.FirstAsync();
        Dictionary<TKey, GrantReturn> results = new(entities.Count);

        foreach ((TKey key, Entity entity) in entities)
        {
            GrantReturn result = TryIncrementBuildTimePoint(userData, entity.Quantity)
                ? GrantReturn.Added()
                : GrantReturn.Limit();

            results[key] = result;
        }

        return results;
    }

    public async Task<GrantReturn> Grant(Entity entity)
    {
        DbPlayerUserData userData = await apiContext.PlayerUserData.FirstAsync();

        return TryIncrementBuildTimePoint(userData, entity.Quantity)
            ? GrantReturn.Added()
            : GrantReturn.Limit();
    }

    private static bool TryIncrementBuildTimePoint(DbPlayerUserData userData, int quantity)
    {
        if (userData.BuildTimePoint + quantity > MaxHammers)
        {
            return false;
        }

        userData.BuildTimePoint += quantity;
        return true;
    }
}
