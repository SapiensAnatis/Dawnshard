using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared;

namespace DragaliaAPI.Database.Repositories;

[Obsolete(ObsoleteReasons.BaaS)]
public interface IDeviceAccountRepository : IBaseRepository
{
    Task AddNewDeviceAccount(string id, string hashedPassword);

    Task<DbDeviceAccount?> GetDeviceAccountById(string id);
}
