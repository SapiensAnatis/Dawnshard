using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.Parties;

public class PartyPowerRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : IPartyPowerRepository
{
    public async Task<DbPartyPower> GetPartyPowerAsync()
    {
        return await apiContext.PartyPowers.FindAsync(playerIdentityService.ViewerId)
            ?? apiContext
                .PartyPowers.Add(new DbPartyPower { ViewerId = playerIdentityService.ViewerId })
                .Entity;
    }

    public async Task<int> GetMaxPartyPowerAsync()
    {
        DbPartyPower dbPower = await this.GetPartyPowerAsync();
        return dbPower.MaxPartyPower;
    }

    public async Task SetMaxPartyPowerAsync(int power)
    {
        DbPartyPower dbPower = await this.GetPartyPowerAsync();
        if (power > dbPower.MaxPartyPower)
        {
            dbPower.MaxPartyPower = power;
        }
    }
}
