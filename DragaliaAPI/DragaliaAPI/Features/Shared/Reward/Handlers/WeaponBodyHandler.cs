using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

public class WeaponBodyHandler(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    : IBatchRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.WeaponBody];

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        List<WeaponBodies> targetIds = entities.Select(x => (WeaponBodies)x.Value.Id).ToList();

        HashSet<WeaponBodies> owned = await apiContext
            .PlayerWeapons.Where(x =>
                x.ViewerId == playerIdentityService.ViewerId && targetIds.Contains(x.WeaponBodyId)
            )
            .Select(x => x.WeaponBodyId)
            .ToHashSetAsync();

        Dictionary<TKey, GrantReturn> resultDict = new(entities.Count);

        foreach ((TKey key, Entity entity) in entities)
        {
            WeaponBodies weaponId = (WeaponBodies)entity.Id;

            Debug.Assert(MasterAsset.WeaponBody[weaponId].IsPlayable);

            if (
                owned.Contains(weaponId)
                || apiContext.PlayerWeapons.Local.Any(x => x.WeaponBodyId == weaponId)
            )
            {
                // The WeaponBody asset contains 'duplicate entity' information, but attempting to return a
                // converted entity result doesn't work properly - it brings up an empty window and has text
                // about adventurers.
                resultDict.Add(key, GrantReturn.Discarded());
            }
            else
            {
                apiContext.PlayerWeapons.Add(
                    new DbWeaponBody()
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        WeaponBodyId = weaponId,
                        LimitBreakCount = entity.LimitBreakCount ?? 0,
                        BuildupCount = entity.BuildupCount ?? 0,
                        EquipableCount = entity.EquipableCount ?? 0,
                    }
                );

                resultDict.Add(key, GrantReturn.Added());
            }
        }

        return resultDict;
    }
}
