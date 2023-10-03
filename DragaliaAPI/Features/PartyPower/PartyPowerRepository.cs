using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.PartyPower;

public class PartyPowerRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    IMissionProgressionService missionProgressionService
) : IPartyPowerRepository
{
    public async Task<DbPartyPower> GetPartyPowerAsync()
    {
        return await apiContext.PartyPowers.FindAsync(playerIdentityService.AccountId)
            ?? apiContext.PartyPowers
                .Add(new DbPartyPower { DeviceAccountId = playerIdentityService.AccountId })
                .Entity;
    }

    public async Task<int> GetMaxPartyPowerAsync()
    {
        DbPartyPower dbPower = await GetPartyPowerAsync();
        return dbPower.MaxPartyPower;
    }

    public async Task SetMaxPartyPowerAsync(int power)
    {
        DbPartyPower dbPower = await GetPartyPowerAsync();
        if (power > dbPower.MaxPartyPower)
        {
            dbPower.MaxPartyPower = power;
            missionProgressionService.OnPartyPowerReached(power);
        }
    }
}
