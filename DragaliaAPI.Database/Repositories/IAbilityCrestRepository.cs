using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Database.Repositories;

public interface IAbilityCrestRepository
{
    IQueryable<DbAbilityCrest> AbilityCrests { get; }
    IQueryable<DbAbilityCrestSet> AbilityCrestSets { get; }
    Task Add(AbilityCrests abilityCrestId);
    Task<DbAbilityCrest?> FindAsync(AbilityCrests abilityCrestId);
    Task AddOrUpdateSet(DbAbilityCrestSet abilityCrestSet);
    Task<DbAbilityCrestSet?> FindSetAsync(int abilityCrestSetNo);
}
