using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

public partial class DragonHandler(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    TimeProvider timeProvider,
    ILogger<DragonHandler> logger
) : IRewardHandler, IBatchRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.Dragon];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        int storageSpace = await apiContext
            .PlayerUserData.Select(x => x.MaxDragonQuantity)
            .FirstAsync();
        int dragonCount = await apiContext.PlayerDragonData.CountAsync();

        if (dragonCount >= storageSpace)
        {
            return GrantReturn.Limit();
        }

        apiContext.PlayerDragonData.Add(
            new DbPlayerDragonData()
            {
                ViewerId = playerIdentityService.ViewerId,
                DragonId = (Dragons)entity.Id,
                GetTime = timeProvider.GetUtcNow(),
            }
        );

        return GrantReturn.Added();
    }

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        int storageSpace = await apiContext
            .PlayerUserData.Select(x => x.MaxDragonQuantity)
            .FirstAsync();
        int dragonCount = await apiContext.PlayerDragonData.CountAsync();

        List<Dragons> ownedReliabilities = await apiContext
            .PlayerDragonReliability.Where(x =>
                entities.Values.Select(y => (Dragons)y.Id).Contains(x.DragonId)
            )
            .Select(x => x.DragonId)
            .ToListAsync();

        DateTimeOffset getTime = timeProvider.GetUtcNow();

        Dictionary<TKey, GrantReturn> resultDict = [];

        foreach ((TKey key, Entity entity) in entities)
        {
            Dragons dragon = (Dragons)entity.Id;

            if (!Enum.IsDefined(dragon))
            {
                Log.InvalidDragonId(logger, entity, dragon);
                resultDict.Add(key, GrantReturn.FailError());
                continue;
            }

            if (dragonCount >= storageSpace)
            {
                resultDict.Add(key, GrantReturn.Limit());
                continue;
            }

            apiContext.PlayerDragonData.Add(
                new DbPlayerDragonData()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    DragonId = dragon,
                    GetTime = getTime,
                }
            );

            if (!ownedReliabilities.Contains(dragon))
            {
                apiContext.PlayerDragonReliability.Add(
                    new DbPlayerDragonReliability(playerIdentityService.ViewerId, dragon)
                );
                ownedReliabilities.Add(dragon);
            }

            resultDict.Add(key, GrantReturn.Added());
            dragonCount++;
        }

        return resultDict;
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Error,
            "Entity {Entity} is not a valid dragon entity: {DragonId} is not a dragon ID"
        )]
        public static partial void InvalidDragonId(ILogger logger, Entity entity, Dragons dragonId);
    }
}
