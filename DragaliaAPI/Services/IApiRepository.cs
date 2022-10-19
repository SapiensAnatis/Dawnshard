using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;

namespace DragaliaAPI.Services;

public interface IApiRepository
{
    Task AddNewDeviceAccount(string id, string hashedPassword);

    Task<DbDeviceAccount?> GetDeviceAccountById(string id);

    Task CreateNewSavefile(string deviceAccountId);

    IQueryable<DbPlayerUserData> GetPlayerInfo(string deviceAccountId);

    Task<DbPlayerUserData> UpdateTutorialStatus(string deviceAccountId, int newStatus);

    Task UpdateName(string deviceAccountId, string newName);

    Task<ISet<int>> getTutorialFlags(string deviceAccountId);

    Task setTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags);

    Task<bool> CheckHasDragon(string deviceAccountId, int dragonId);

    Task<bool> CheckHasChara(string deviceAccountId, int charaId);

    Task<DbPlayerCharaData> AddChara(string deviceAccountId, int id, int rarity);

    Task<DbPlayerDragonData> AddDragon(string deviceAccountId, int id, int rarity);

    IQueryable<DbPlayerCharaData> GetCharaData(string deviceAccountId);

    IQueryable<DbPlayerDragonData> GetDragonData(string deviceAccountId);

    IQueryable<DbParty> GetParties(string deviceAccountId);
}
