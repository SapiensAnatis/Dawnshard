using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class PartyRepository : IPartyRepository
{
    private readonly ApiContext apiContext;

    public PartyRepository(ApiContext apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbParty> GetParties(string deviceAccountId)
    {
        return apiContext.PlayerParties
            .Include(x => x.Units.OrderBy(x => x.Id))
            .Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public async Task SetParty(string deviceAccountId, DbParty newParty)
    {
        DbParty existingParty = await apiContext.PlayerParties
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

        await apiContext.SaveChangesAsync();
    }
}
