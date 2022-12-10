using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Utils;

/// <summary>
/// Extension methods to add or update values. Used in savefile imports.
/// </summary>
public static class AddOrUpdateExtensions
{
    public static async Task AddOrUpdateRange<TEntity>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TEntity> newEntities,
        Func<TEntity, object[]> keyValuePredicate
    ) where TEntity : class, IDbHasAccountId
    {
        foreach (TEntity entity in newEntities)
        {
            TEntity? dbEntity = await dbSet.FindAsync(keyValuePredicate.Invoke(entity));

            if (dbEntity is null)
            {
                dbSet.Add(entity);
            }
            else
            {
                if (dbEntity.DeviceAccountId != entity.DeviceAccountId)
                {
                    throw new ArgumentException(
                        $"Failed to update entity {dbEntity} belonging to other deviceAccount {dbEntity.DeviceAccountId}"
                    );
                }

                // TODO: Why can't I do dbEntity = entity and have the change tracker pick it up
                dbSet.Remove(dbEntity);
                dbSet.Add(entity);
            }
        }
    }
}
