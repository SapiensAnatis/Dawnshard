using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IAbilityCrestRepository
{
    IQueryable<DbAbilityCrest> AbilityCrests { get; }
    IQueryable<DbAbilityCrestSet> AbilityCrestSets { get; }

    Task<DbAbilityCrest?> FindAsync(AbilityCrests abilityCrestId);

    Task AddOrUpdateSet(DbAbilityCrestSet abilityCrestSet);
    Task<DbAbilityCrestSet?> FindSetAsync(int abilityCrestSetNo);

    /// <summary>
    /// Adds an ability crest.
    /// </summary>
    /// <param name="abilityCrestId">The ability crest ID.</param>
    /// <param name="limitBreakCount">The limit break count.</param>
    /// <param name="buildupCount"></param>
    /// <param name="equipableCount"></param>
    /// <returns>The task.</returns>
    /// <remarks>Issues a warning if a duplicate is attempted to be added.</remarks>
    Task Add(
        AbilityCrests abilityCrestId,
        int? limitBreakCount = null,
        int? buildupCount = null,
        int? equipableCount = null
    );
}
