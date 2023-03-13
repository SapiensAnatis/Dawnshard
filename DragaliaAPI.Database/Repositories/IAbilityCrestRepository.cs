using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Database.Repositories;

public interface IAbilityCrestRepository
{
    IQueryable<DbAbilityCrest> AbilityCrests { get; }
    Task Add(AbilityCrests abilityCrestId);
}
