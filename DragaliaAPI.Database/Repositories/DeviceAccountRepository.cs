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
public class DeviceAccountRepository : BaseRepository, IDeviceAccountRepository
{
    private readonly ApiContext apiContext;
    private readonly ICharaDataService charaDataService;

    public DeviceAccountRepository(ApiContext apiContext, ICharaDataService charaDataService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.charaDataService = charaDataService;
    }

    public async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
    }

    public async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await apiContext.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateNewSavefile(string deviceAccountId)
    {
        DbPlayerUserData userData = DbSavefileUserDataFactory.Create(deviceAccountId);

#if SKIP_TUTORIAL
        userData.TutorialStatus = 10151;
#endif

        await apiContext.PlayerUserData.AddAsync(userData);

        await apiContext.PlayerCharaData.AddAsync(
            DbPlayerCharaDataFactory.Create(
                deviceAccountId,
                charaDataService.GetData(Charas.ThePrince)
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
                        new() { UnitNo = 1, CharaId = Charas.ThePrince },
                        new() { UnitNo = 2, CharaId = Charas.Empty },
                        new() { UnitNo = 3, CharaId = Charas.Empty },
                        new() { UnitNo = 4, CharaId = Charas.Empty }
                    }
                }
            );
        }

        await apiContext.PlayerParties.AddRangeAsync(defaultParties);
    }
}
