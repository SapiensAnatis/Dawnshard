using System.Collections.Immutable;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
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

    public async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await _apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
        await _apiContext.SaveChangesAsync();
    }

    public async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await _apiContext.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddNewPlayerInfo(string deviceAccountId)
    {
        await _apiContext.PlayerUserData.AddAsync(
            DbSavefileUserDataFactory.Create(deviceAccountId)
        );
        await _apiContext.SaveChangesAsync();
    }

    public IQueryable<DbPlayerUserData> GetPlayerInfo(string deviceAccountId)
    {
        IQueryable<DbPlayerUserData> infoQuery = _apiContext.PlayerUserData.Where(
            x => x.DeviceAccountId == deviceAccountId
        );

        return infoQuery;
    }

    public async Task<DbPlayerUserData> UpdateTutorialStatus(string deviceAccountId, int newStatus)
    {
        DbPlayerUserData userData =
            await _apiContext.PlayerUserData.FindAsync(deviceAccountId)
            ?? throw new NullReferenceException("Savefile lookup failed");
        userData.TutorialStatus = newStatus;
        await _apiContext.SaveChangesAsync();
        return userData;
    }

    public async Task UpdateName(string deviceAccountId, string newName)
    {
        DbPlayerUserData userData =
            await _apiContext.PlayerUserData.FindAsync(deviceAccountId)
            ?? throw new NullReferenceException("Savefile lookup failed");
        userData.Name = newName;
        await _apiContext.SaveChangesAsync();
    }

    public virtual async Task<ISet<int>> getTutorialFlags(string deviceAccountId)
    {
        DbPlayerUserData? saveFileUserData = await _apiContext.PlayerUserData.FindAsync(
            deviceAccountId
        );
        if (saveFileUserData == null)
        {
            throw new Exception($"No SaveFileData found for Account-Id: {deviceAccountId}");
        }
        int flags_ = saveFileUserData.TutorialFlag;
        return TutorialFlagUtil.ConvertIntToFlagIntList(flags_);
    }

    public virtual async Task setTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags)
    {
        DbPlayerUserData? saveFileUserData = await _apiContext.PlayerUserData.FindAsync(
            deviceAccountId
        );
        if (saveFileUserData == null)
        {
            throw new Exception($"No SaveFileData found for Account-Id: {deviceAccountId}");
        }
        saveFileUserData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(
            tutorialFlags,
            saveFileUserData.TutorialFlag
        );
        int udpatedRows = await _apiContext.SaveChangesAsync();
    }

    public async Task<bool> CheckHasChara(string deviceAccountId, int charaId)
    {
        return await _apiContext.PlayerCharaData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .AnyAsync(x => (int)x.CharaId == charaId);
    }

    public async Task<bool> CheckHasDragon(string deviceAccountId, int dragonId)
    {
        return await _apiContext.PlayerDragonData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .AnyAsync(x => (int)x.DragonId == dragonId);
    }

    public async Task<DbPlayerCharaData> AddChara(string deviceAccountId, int id, int rarity)
    {
        DbPlayerCharaData dbEntry = DbPlayerCharaDataFactory.Create(deviceAccountId, id, rarity);
        await _apiContext.PlayerCharaData.AddAsync(dbEntry);
        await _apiContext.SaveChangesAsync();
        return dbEntry;
    }

    public async Task<DbPlayerDragonData> AddDragon(string deviceAccountId, int id, int rarity)
    {
        DbPlayerDragonData dbEntry = DbPlayerDragonDataFactory.Create(deviceAccountId, id, rarity);
        await _apiContext.PlayerDragonData.AddAsync(dbEntry);
        await _apiContext.SaveChangesAsync();
        return dbEntry;
    }
}
