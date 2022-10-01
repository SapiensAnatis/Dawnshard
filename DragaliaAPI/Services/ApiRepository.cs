using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

/// <summary>
/// Abstraction upon the context to access the database without exposing all the methods of DbContext.
/// Use this instead of directly injecting the context.
/// </summary>
public class ApiRepository : IApiRepository
{
    private readonly ApiContext _apiContext;

    public ApiRepository(ApiContext context)
    {
        _apiContext = context;
    }

    public virtual async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await _apiContext.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await _apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
        await _apiContext.SaveChangesAsync();
    }

    public virtual async Task AddNewPlayerSavefile(string deviceAccountId)
    {
        await _apiContext.PlayerSavefiles.AddAsync(new DbSavefilePlayerInfo() { DeviceAccountId = deviceAccountId });
        await _apiContext.SaveChangesAsync();
    }

    public virtual IQueryable<DbSavefilePlayerInfo> GetPlayerInfo(string deviceAccountId)
    {
        return _apiContext.PlayerSavefiles.Where(x => x.DeviceAccountId == deviceAccountId);
    }
}