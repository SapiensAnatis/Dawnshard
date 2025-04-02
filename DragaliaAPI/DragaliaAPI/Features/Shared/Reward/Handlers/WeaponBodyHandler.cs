using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
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
        Dictionary<WeaponBodies, TKey> inverseKeyLookup = entities.ToDictionary(
            x => (WeaponBodies)x.Value.Id,
            x => x.Key
        );

        List<DbWeaponBody> toAdd = entities
            .Values.Select(x => new DbWeaponBody()
            {
                ViewerId = playerIdentityService.ViewerId,
                WeaponBodyId = (WeaponBodies)x.Id,
            })
            .ToList();

        var mergeResult = apiContext
            .PlayerWeapons.ToLinqToDBTable()
            .Merge()
            .Using(entities.Values)
            .On(
                (dbEntry, entity) =>
                    dbEntry.ViewerId == playerIdentityService.ViewerId
                    && dbEntry.WeaponBodyId == (WeaponBodies)entity.Id
            )
            .InsertWhenNotMatched(entity => new DbWeaponBody()
            {
                ViewerId = playerIdentityService.ViewerId,
                WeaponBodyId = (WeaponBodies)entity.Id,
            })
            .MergeWithOutputAsync(
                (string action, DbWeaponBody old, DbWeaponBody _) =>
                    new { Action = action, Id = old.WeaponBodyId }
            );

        var resultDict = new Dictionary<TKey, GrantReturn>(entities.Count);

        await foreach (var result in mergeResult)
        {
            TKey key = inverseKeyLookup[result.Id];
            resultDict[key] =
                result.Action == "inserted" ? GrantReturn.Added() : GrantReturn.Converted(null);
        }

        return new Dictionary<TKey, GrantReturn>();
    }
}
