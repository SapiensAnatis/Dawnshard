using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Models.Database
{
    /// <summary>
    /// Abstraction upon the context to access the database without exposing all the methods of DbContext.
    /// Use this instead of directly injecting the context.
    /// </summary>
    public class ApiRepository : IApiRepository
    {
        private readonly ApiContext _context;

        public ApiRepository(ApiContext context)
        {
            _context = context;
        }

        public virtual async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
        {
            return await _context.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task AddNewDeviceAccount(string id, string hashedPassword)
        {
            await _context.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
            await _context.SaveChangesAsync();
        }

        public virtual async Task AddNewPlayerSavefile(string deviceAccountId)
        {
            await _context.PlayerSavefiles.AddAsync(new DbPlayerSavefile() { DeviceAccountId = deviceAccountId });
            await _context.SaveChangesAsync();
        }

        public virtual async Task<DbPlayerSavefile> GetSavefileByDeviceAccountId(string deviceAccountId)
        {
            return await _context.PlayerSavefiles.FirstAsync(x => x.DeviceAccountId == deviceAccountId);
        }
    }
}
