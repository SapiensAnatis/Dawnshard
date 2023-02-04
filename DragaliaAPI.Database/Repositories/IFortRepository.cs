using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository
{
    IQueryable<DbFortBuild> GetBuilds(string accountId);
    IQueryable<DbFortBuild> Builds { get; }
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
}
