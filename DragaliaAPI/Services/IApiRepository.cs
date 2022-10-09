using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;

namespace DragaliaAPI.Services;

public interface IApiRepository
{
    Task AddNewDeviceAccount(string id, string hashedPassword);
    Task<DbDeviceAccount?> GetDeviceAccountById(string id);
    Task AddNewPlayerInfo(string deviceAccountId);
    IQueryable<DbSavefileUserData> GetPlayerInfo(string deviceAccountId);
    Task<ISet<int>> getTutorialFlags(string deviceAccountId);
    Task setTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags);
}