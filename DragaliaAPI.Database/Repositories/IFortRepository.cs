using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository : IBaseRepository
{
    IQueryable<DbFortBuild> GetBuilds(string accountId);
    IQueryable<DbFortBuild> Builds { get; }
    IQueryable<DbFortDetail> Details { get; }
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
    Task<bool> InitFortDetail(string deviceAccountId);
    Task UpdateFortCarpenterNum(string accountId, int carpenter_num);
}
