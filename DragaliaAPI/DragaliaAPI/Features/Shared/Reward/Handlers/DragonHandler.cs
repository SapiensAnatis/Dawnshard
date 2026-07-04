using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

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

        DragonId dragon = (DragonId)entity.Id;
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

        apiContext.PlayerDragonData.Add(new(playerIdentityService.ViewerId, (DragonId)entity.Id));

        if (
            !apiContext.PlayerDragonReliability.Local.Any(x => x.DragonId == dragon)
            && !await apiContext.PlayerDragonReliability.AnyAsync(x => x.DragonId == dragon)
        )
        {
            DbPlayerDragonReliability reliability = new(playerIdentityService.ViewerId, dragon);
            apiContext.PlayerDragonReliability.Add(reliability);
            this.AddDefaultLevelStories(dragon, reliability.Level);
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

        HashSet<DragonId> ownedReliabilities = await apiContext
            .PlayerDragonReliability.Where(x =>
                entities.Values.Select(y => (DragonId)y.Id).Contains(x.DragonId)
            )
            .Select(x => x.DragonId)
            .ToHashSetAsync();

        Dictionary<TKey, GrantReturn> resultDict = [];

        foreach ((TKey key, Entity entity) in entities)
        {
            DragonId dragon = (DragonId)entity.Id;

            if (!Enum.IsDefined(dragon))
            {
                Log.InvalidDragonId(logger, entity, dragon);
                resultDict.Add(key, GrantReturn.FailError());
                continue;
            }

            if (dragonCount + entity.Quantity > storageSpace)
            {
                resultDict.Add(key, GrantReturn.Limit());
                continue;
            }

            for (int i = 0; i < entity.Quantity; i++)
            {
                apiContext.PlayerDragonData.Add(new(playerIdentityService.ViewerId, dragon));
            }

            if (
                !ownedReliabilities.Contains(dragon)
                && !apiContext.PlayerDragonReliability.Local.Any(x => x.DragonId == dragon)
            )
            {
                DbPlayerDragonReliability reliability = new(playerIdentityService.ViewerId, dragon);
                apiContext.PlayerDragonReliability.Add(reliability);
                ownedReliabilities.Add(dragon);
                this.AddDefaultLevelStories(dragon, reliability.Level);
            }

            resultDict.Add(key, GrantReturn.Added());
            dragonCount++;
        }

        return resultDict;
    }

    private void AddDefaultLevelStories(DragonId dragon, int reliabilityLevel)
    {
        if (
            reliabilityLevel < 5
            || !MasterAsset.DragonStories.TryGetValue((int)dragon, out StoryData? storyData)
        )
        {
            return;
        }

        Debug.Assert(
            storyData.StoryIds.Length == 2,
            "Expected all dragons to have exactly two stories"
        );

        int storiesToUnlock = reliabilityLevel >= 15 ? 2 : 1;

        for (int i = 0; i < storiesToUnlock; i++)
        {
            apiContext.PlayerStoryState.Add(
                new DbPlayerStoryState()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    StoryType = StoryTypes.Dragon,
                    StoryId = storyData.StoryIds[i],
                }
            );
        }
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Error,
            "Entity {Entity} is not a valid dragon entity: {DragonId} is not a dragon ID"
        )]
        public static partial void InvalidDragonId(
            ILogger logger,
            Entity entity,
            DragonId dragonId
        );
    }
}
