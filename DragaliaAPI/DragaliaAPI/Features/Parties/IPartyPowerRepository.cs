using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Parties;

public interface IPartyPowerRepository
{
    Task<DbPartyPower> GetPartyPowerAsync();

    Task<int> GetMaxPartyPowerAsync();
    Task SetMaxPartyPowerAsync(int power);
}
