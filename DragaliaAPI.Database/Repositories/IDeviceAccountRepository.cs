using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

[Obsolete("Used by pre-BaaS login flow")]
public interface IDeviceAccountRepository : IBaseRepository
{
    Task AddNewDeviceAccount(string id, string hashedPassword);

    Task<DbDeviceAccount?> GetDeviceAccountById(string id);
}
