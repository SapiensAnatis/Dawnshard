using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Models.Dragalia
{
    /// <summary>
    /// Abstraction on DbPlayerSavefile -- repository-like object which performs queries
    /// and provides an object-oriented interface to player save data.
    /// 
    /// Uses context rather than ApiRepository as it would make no sense to add methods to
    /// the ApiRepository which are only used here, and this is already very similar to a 
    /// repository.
    /// 
    /// This is not really a service but rather an object that contains a device account,
    /// which it uses to lookup the relevant savefile.
    /// </summary>
    public class PlayerSavefile
    {
        private readonly ApiContext _context;
        private readonly DeviceAccount _deviceAccount;
        public PlayerSavefile(DeviceAccount deviceAccount, ApiContext context)
        {
            _context = context;
            _deviceAccount = deviceAccount;
        }

        private DbPlayerSavefile _dbSavefile
        {
            get
            {
                return _context.PlayerSavefiles.First(x => x.DeviceAccountId == _deviceAccount.id);
            }
        }

        public long ViewerId
        {
            get
            {
                return _dbSavefile.ViewerId;
            }
        }
    }
}
