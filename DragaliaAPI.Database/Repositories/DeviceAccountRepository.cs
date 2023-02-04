using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Database.Repositories;

/// <summary>
/// Repository to
/// </summary>
[Obsolete(ObsoleteReasons.BaaS)]
public class DeviceAccountRepository : BaseRepository, IDeviceAccountRepository
{
    private readonly ApiContext apiContext;

    public DeviceAccountRepository(ApiContext apiContext)
        : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
    }

    public async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await apiContext.DeviceAccounts.SingleOrDefaultAsync(x => x.Id == id);
    }
}
