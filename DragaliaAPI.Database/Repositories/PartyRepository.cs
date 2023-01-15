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

    public IQueryable<DbPartyUnit> GetPartyUnits(string deviceAccountId, IEnumerable<int> partyNos)
    {
        return apiContext.PlayerPartyUnits
            .Where(
                x =>
                    x.DeviceAccountId == deviceAccountId
                    && (
                        x.PartyNo == partyNos.ElementAt(0)
                        || x.PartyNo == partyNos.ElementAtOrDefault(1)
                    )
            )
            .OrderBy(x => x.PartyNo == partyNos.First())
            .ThenBy(x => x.UnitNo);
    }

    public async Task SetParty(string deviceAccountId, DbParty newParty)
    {
        // TODO: this method executes a query where it deletes the old units and adds the new ones
        // Could it be optimized by updating the units instead? Anticipate that most changes will be small
        DbParty existingParty = await apiContext.PlayerParties
            .Where(x => x.DeviceAccountId == deviceAccountId && x.PartyNo == newParty.PartyNo)
            .Include(x => x.Units)
            .SingleAsync();

        existingParty.PartyName = newParty.PartyName;
        existingParty.Units = CleanUnitList(newParty.Units);

        apiContext.Entry(existingParty).State = EntityState.Modified;
    }

    public async Task UpdatePartyName(string deviceAccountId, int partyNo, string newName)
    {
        DbParty existingParty = await apiContext.PlayerParties
            .Where(x => x.DeviceAccountId == deviceAccountId && x.PartyNo == partyNo)
            .Include(x => x.Units) // Need to return full unit list in update_data_list
            .SingleAsync();

        existingParty.PartyName = newName;
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
