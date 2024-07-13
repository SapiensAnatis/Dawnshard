using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class DiamantiumHandler(ApiContext apiContext) : IRewardHandler, IBatchRewardHandler
{
    private const int MaxDiamantium = 9_999_999;

    public IReadOnlyList<EntityTypes> SupportedTypes { get; } =
        [EntityTypes.FreeDiamantium, EntityTypes.PaidDiamantium];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        DbPlayerDiamondData diamondData = await apiContext
            .PlayerDiamondData.AsTracking()
            .FirstAsync();

        return UpdateDiamond(entity, diamondData);
    }

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        Dictionary<TKey, GrantReturn> result = new(entities.Count);

        DbPlayerDiamondData diamondData = await apiContext
            .PlayerDiamondData.AsTracking()
            .FirstAsync();

        foreach ((TKey key, Entity entity) in entities)
        {
            result[key] = UpdateDiamond(entity, diamondData);
        }

        return result;
    }

    private static GrantReturn UpdateDiamond(Entity entity, DbPlayerDiamondData diamondData)
    {
        if (entity.Type == EntityTypes.PaidDiamantium)
        {
            if (diamondData.PaidDiamond + entity.Quantity > MaxDiamantium)
            {
                return GrantReturn.Limit();
            }

            diamondData.PaidDiamond += entity.Quantity;
            return GrantReturn.Added();
        }
        else if (entity.Type == EntityTypes.FreeDiamantium)
        {
            if (diamondData.FreeDiamond + entity.Quantity > MaxDiamantium)
            {
                return GrantReturn.Limit();
            }

            diamondData.FreeDiamond += entity.Quantity;
            return GrantReturn.Added();
        }
        else
        {
            throw new UnreachableException("Invalid entity type");
        }
    }
}
