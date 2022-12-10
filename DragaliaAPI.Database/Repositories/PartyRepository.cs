using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class PartyRepository : BaseRepository, IPartyRepository
{
    private readonly ApiContext apiContext;

    public PartyRepository(ApiContext apiContext) : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbParty> GetParties(string deviceAccountId)
    {
        return apiContext.PlayerParties
            .Include(x => x.Units.OrderBy(x => x.UnitNo))
            .Where(x => x.DeviceAccountId == deviceAccountId);
    }

    public async Task SetParty(string deviceAccountId, DbParty newParty)
    {
        DbParty existingParty = await apiContext.PlayerParties
            .Include(x => x.Units)
            .Where(x => x.DeviceAccountId == deviceAccountId && x.PartyNo == newParty.PartyNo)
            .SingleAsync();

        foreach (DbPartyUnit newUnit in CleanUnitList(newParty.Units))
        {
            DbPartyUnit existingUnit = existingParty.Units.Single(x => x.UnitNo == newUnit.UnitNo);

            existingUnit.EquipDragonKeyId = newUnit.EquipDragonKeyId;
        }

        await apiContext.SaveChangesAsync();

        apiContext.PlayerParties.Entry(existingParty).State = EntityState.Modified;
    }

    private static ICollection<DbPartyUnit> CleanUnitList(ICollection<DbPartyUnit> original)
    {
        // For some reason, pressing 'Optimize' can send a request to /party/set_party_setting with like 8 units in it
        // Take the first one under each number, and fill in blanks if needed.
        List<DbPartyUnit> result = new();
        for (int i = 1; i <= 4; i++)
        {
            result.Add(
                original.FirstOrDefault(x => x.UnitNo == i) ?? new() { UnitNo = i, CharaId = 0 }
            );
        }

        return result;
    }
}
