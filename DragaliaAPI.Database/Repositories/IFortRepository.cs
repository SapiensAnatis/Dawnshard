using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository
{
    IQueryable<DbFortBuild> GetBuilds(string accountId);
}
