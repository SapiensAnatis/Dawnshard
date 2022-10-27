using DragaliaAPI.Models;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Models.Dragalia.Responses;

namespace DragaliaAPI.Services;

/// <summary>
/// Abstraction upon the context to access the database without exposing all the methods of DbContext.
/// Use this instead of directly injecting the context.
/// </summary>
public class ApiRepository : IApiRepository
{
    private readonly ApiContext _apiContext;
    private readonly IUnitDataService _unitDataService;
    private readonly IDragonDataService _dragonDataService;
    private readonly IWebHostEnvironment environment;

    public ApiRepository(
        ApiContext context,
        IUnitDataService unitDataService,
        IDragonDataService dragonDataService,
        IWebHostEnvironment environment
    )
    {
        _apiContext = context;
        _unitDataService = unitDataService;
        _dragonDataService = dragonDataService;
        this.environment = environment;
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

    public async Task CreateNewSavefile(string deviceAccountId)
    {
        DbPlayerUserData userData = DbSavefileUserDataFactory.Create(deviceAccountId);

        if (environment.IsDevelopment())
        {
            // Skip the tutorial
            userData.TutorialStatus = 10151;
        }

        await _apiContext.PlayerUserData.AddAsync(userData);

        await _apiContext.PlayerCharaData.AddAsync(
            DbPlayerCharaDataFactory.Create(
                deviceAccountId,
                _unitDataService.GetData(Charas.ThePrince),
                4
            )
        );

        List<DbParty> defaultParties = new();

        // New savefiles come with 54 parties, for some reason
        for (int i = 1; i <= 54; i++)
        {
            defaultParties.Add(
                new()
                {
                    DeviceAccountId = deviceAccountId,
                    PartyName = "Default",
                    PartyNo = i,
                    Units = new List<DbPartyUnit>()
                    {
                        new() { UnitNo = 1, CharaId = Charas.ThePrince }
                    }
                }
            );
        }

        await _apiContext.PlayerParties.AddRangeAsync(defaultParties);

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

    public async Task<DbPlayerUserData> UpdateTutorialFlag(string deviceAccountId, int flag)
    {
        DbPlayerUserData userData =
            await _apiContext.PlayerUserData.FindAsync(deviceAccountId)
            ?? throw new NullReferenceException("Savefile lookup failed");
        ISet<int> flags = TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag);
        flags.Add(flag);
        userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(flags);
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

    public async Task<ISet<int>> getTutorialFlags(string deviceAccountId)
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

    public async Task setTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags)
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
        return await _apiContext.PlayerDragonReliability
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .AnyAsync(x => (int)x.DragonId == dragonId);
    }

    public async Task<DbPlayerCharaData> AddChara(string deviceAccountId, int id, int rarity)
    {
        DbPlayerCharaData dbEntry = DbPlayerCharaDataFactory.Create(
            deviceAccountId,
            _unitDataService.GetData(id),
            (byte)rarity
        );
        await _apiContext.PlayerCharaData.AddAsync(dbEntry);
        await _apiContext.SaveChangesAsync();
        return dbEntry;
    }

    public async Task<DbPlayerDragonData> AddDragon(string deviceAccountId, int id, int rarity)
    {
        DbPlayerDragonData dbEntry = DbPlayerDragonDataFactory.Create(deviceAccountId, (Dragons)id);
        await _apiContext.PlayerDragonData.AddAsync(dbEntry);
        await _apiContext.SaveChangesAsync();
        return dbEntry;
    }

    public async Task<DbPlayerDragonReliability> AddDragonReliability(
        string deviceAccountId,
        int id
    )
    {
        DbPlayerDragonReliability dbEntry = DbPlayerDragonReliabilityFactory.Create(
            deviceAccountId,
            (Dragons)id
        );
        await _apiContext.PlayerDragonReliability.AddAsync(dbEntry);
        await _apiContext.SaveChangesAsync();
        return dbEntry;
    }

    public async Task<DbPlayerBannerData> AddPlayerBannerData(string deviceAccountId, int bannerId)
    {
        DbPlayerBannerData bannerData = new DbPlayerBannerData()
        {
            DeviceAccountId = deviceAccountId,
            //TODO Probably get all this and more from bannerInfo
            SummonBannerId = bannerId,
            ConsecutionSummonPointsMinDate = DateTimeOffset.UtcNow,
            ConsecutionSummonPointsMaxDate = DateTimeOffset.UtcNow.AddDays(7),
        };
        bannerData = _apiContext.PlayerBannerData.Add(bannerData).Entity;
        await _apiContext.SaveChangesAsync();
        return bannerData;
    }

    public async Task<List<DbPlayerSummonHistory>> GetSummonHistory(string deviceAccountId)
    {
        return await _apiContext.PlayerSummonHistory
            .Where(x => x.DeviceAccountId.Equals(deviceAccountId))
            .ToListAsync();
    }

    public async Task<DbPlayerBannerData> GetPlayerBannerData(string deviceAccountId, int bannerId)
    {
        DbPlayerBannerData bannerData =
            await _apiContext.PlayerBannerData.FirstOrDefaultAsync(
                x => x.DeviceAccountId.Equals(deviceAccountId) && x.SummonBannerId == bannerId
            ) ?? await AddPlayerBannerData(deviceAccountId, bannerId);
        return bannerData;
    }

    public IQueryable<DbPlayerCharaData> GetCharaData(string deviceAccountId)
    {
        return _apiContext.PlayerCharaData.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbPlayerDragonData> GetDragonData(string deviceAccountId)
    {
        return _apiContext.PlayerDragonData.Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public IQueryable<DbParty> GetParties(string deviceAccountId)
    {
        return _apiContext.PlayerParties
            .Include(x => x.Units)
            .Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public async Task SetParty(string deviceAccountId, DbParty newParty)
    {
        DbParty existingParty = await _apiContext.PlayerParties
            .Where(x => x.DeviceAccountId == deviceAccountId && x.PartyNo == newParty.PartyNo)
            .Include(x => x.Units)
            .SingleAsync();

        existingParty.PartyName = newParty.PartyName;

        // For some reason, pressing 'Optimize' sends a request to /party/set_party_setting with like 8 units in it
        // Take the first one under each number
        existingParty.Units.Clear();
        for (int i = 1; i <= 4; i++)
        {
            existingParty.Units.Add(
                newParty.Units.FirstOrDefault(x => x.UnitNo == i)
                    ?? new() { UnitNo = i, CharaId = 0 }
            );
        }

        await _apiContext.SaveChangesAsync();
    }
}
