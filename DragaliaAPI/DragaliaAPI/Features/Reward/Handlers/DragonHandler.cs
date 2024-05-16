using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

public partial class DragonHandler(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<DragonHandler> logger
) : IRewardHandler, IBatchRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.Dragon];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        if (entity.Quantity > 1)
        {
            throw new ArgumentException(
                "Cannot process dragons with quantity >1 in single reward handler due to the possibility of multiple results. Use the batch handler instead.",
                nameof(entity)
            );
        }

        Dragons dragon = (Dragons)entity.Id;
        if (!Enum.IsDefined(dragon))
        {
            Log.InvalidDragonId(logger, entity, dragon);
            return GrantReturn.FailError();
        }

        int storageSpace = await apiContext
            .PlayerUserData.Select(x => x.MaxDragonQuantity)
            .FirstAsync();
        int dragonCount = await apiContext.PlayerDragonData.CountAsync();

        if (dragonCount >= storageSpace)
        {
            return GrantReturn.Limit();
        }

        apiContext.PlayerDragonData.Add(
            new DbPlayerDragonData(playerIdentityService.ViewerId, (Dragons)entity.Id)
        );

        if (
            !apiContext.PlayerDragonReliability.Local.Any(x => x.DragonId == dragon)
            && !await apiContext.PlayerDragonReliability.AnyAsync(x => x.DragonId == dragon)
        )
        {
            apiContext.PlayerDragonReliability.Add(
                new DbPlayerDragonReliability(playerIdentityService.ViewerId, dragon)
            );
        }

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

        HashSet<Dragons> ownedReliabilities = await apiContext
            .PlayerDragonReliability.Where(x =>
                entities.Values.Select(y => (Dragons)y.Id).Contains(x.DragonId)
            )
            .Select(x => x.DragonId)
            .ToHashSetAsync();

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
                new DbPlayerDragonData(playerIdentityService.ViewerId, dragon)
            );

            if (
                !ownedReliabilities.Contains(dragon)
                && !apiContext.PlayerDragonReliability.Local.Any(x => x.DragonId == dragon)
            )
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
