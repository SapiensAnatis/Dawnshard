using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IUnitRepository : IBaseRepository
{
    IQueryable<DbPlayerCharaData> GetAllCharaData(string deviceAccountId);
    IQueryable<DbPlayerDragonData> GetAllDragonData(string deviceAccountId);

    Task<bool> CheckHasCharas(string deviceAccountId, IEnumerable<Charas> idList);

    Task<bool> CheckHasDragons(string deviceAccountId, IEnumerable<Dragons> idList);

    Task<IEnumerable<(Charas id, bool isNew)>> AddCharas(
        string deviceAccountId,
        IEnumerable<Charas> idList
    );

    Task<(
        IEnumerable<DbPlayerDragonData> newDragons,
        IEnumerable<DbPlayerDragonReliability> newReliabilities
    )> AddDragons(string deviceAccountId, IEnumerable<Dragons> idList);
    Task<DbSetUnit> GetOrCreateCharaSetData(string deviceAccountId, Charas charaId, int setNo);
    Task<IEnumerable<(Dragons id, bool isNew)>> AddDragons(
        string deviceAccountId,
        IEnumerable<Dragons> idList
    );
}
