using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

internal sealed class DmodePointRewardHandler(
    ApiContext apiContext,
    IDmodeRepository dmodeRepository
) : IBatchRewardHandler
{
    private const int MaxDmodePoint = 999_999_999;

    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.DmodePoint];

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        DbPlayerDmodeInfo info = await this.GetOrCreateDmodeInfo();
        Dictionary<TKey, GrantReturn> result = new(entities.Count);

        foreach ((TKey key, Entity entity) in entities)
        {
            if (entity.Id == (int)DmodePoint.Point1)
            {
                if (info.Point1Quantity + entity.Quantity > MaxDmodePoint)
                {
                    result.Add(key, GrantReturn.Limit());
                    continue;
                }

                info.Point1Quantity += entity.Quantity;
                result.Add(key, GrantReturn.Added());
            }
            else if (entity.Id == (int)DmodePoint.Point2)
            {
                if (info.Point2Quantity + entity.Quantity > MaxDmodePoint)
                {
                    result.Add(key, GrantReturn.Limit());
                    continue;
                }

                info.Point2Quantity += entity.Quantity;
                result.Add(key, GrantReturn.Added());
            }
            else
            {
                throw new InvalidOperationException("Invalid entity ID for DmodePoint!");
            }
        }

        return result;
    }

    private async Task<DbPlayerDmodeInfo> GetOrCreateDmodeInfo()
    {
        DbPlayerDmodeInfo? info = await apiContext.PlayerDmodeInfos.FirstOrDefaultAsync();

        if (info is null)
        {
            dmodeRepository.InitializeForPlayer();
            info = apiContext.PlayerDmodeInfos.Local.First();
        }

        return info;
    }
}
