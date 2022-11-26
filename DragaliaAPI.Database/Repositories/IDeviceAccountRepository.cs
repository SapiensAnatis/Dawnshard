using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IDeviceAccountRepository : IBaseRepository
{
    Task AddNewDeviceAccount(string id, string hashedPassword);

    Task<DbDeviceAccount?> GetDeviceAccountById(string id);

    Task CreateNewSavefile(string deviceAccountId);
}
