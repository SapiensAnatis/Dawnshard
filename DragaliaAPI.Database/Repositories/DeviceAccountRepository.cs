#define SKIP_TUTORIAL

using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Factories;

namespace DragaliaAPI.Database.Repositories;

/// <summary>
/// Repository to
/// </summary>
public class DeviceAccountRepository : IDeviceAccountRepository
{
    private readonly ApiContext _apiContext;
    private readonly ICharaDataService _charaDataService;

    public DeviceAccountRepository(ApiContext context, ICharaDataService charaDataService)
    {
        _apiContext = context;
        _charaDataService = charaDataService;
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

#if SKIP_TUTORIAL
        userData.TutorialStatus = 10151;
#endif

        await _apiContext.PlayerUserData.AddAsync(userData);

        await _apiContext.PlayerCharaData.AddAsync(
            DbPlayerCharaDataFactory.Create(
                deviceAccountId,
                _charaDataService.GetData(Charas.ThePrince)
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
}
