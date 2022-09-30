using DragaliaAPI.Models.Database;

namespace DragaliaAPI.Services;

public interface IApiRepository
{
    Task AddNewDeviceAccount(string id, string hashedPassword);
    Task<DbDeviceAccount?> GetDeviceAccountById(string id);
    Task AddNewPlayerSavefile(string deviceAccountId);
    IQueryable<DbPlayerSavefile> GetSavefile(string deviceAccountId);
}