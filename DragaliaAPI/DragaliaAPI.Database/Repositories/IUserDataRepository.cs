using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IUserDataRepository : IBaseRepository
{
    IQueryable<DbPlayerUserData> UserData { get; }

    Task<DbPlayerUserData> GetUserDataAsync();
    Task<DateTimeOffset> GetFortOpenTimeAsync();

    IQueryable<DbPlayerUserData> GetViewerData(long viewerId);
    Task GiveWyrmite(int quantity);
    Task SetMainPartyNo(int partyNo);
    Task UpdateName(string newName);
    Task UpdateSaveImportTime();
    Task UpdateCoin(long offset);
    Task<bool> CheckCoin(long quantity);
    Task UpdateDewpoint(int quantity);
    Task<bool> CheckDewpoint(int quantity);
    Task SetDewpoint(int quantity);
    IQueryable<DbPlayerUserData> GetMultipleViewerData(IEnumerable<long> viewerIds);
}
