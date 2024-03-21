using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Repositories;

public class PartyRepository : BaseRepository, IPartyRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public PartyRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbParty> Parties =>
        this
            .apiContext.PlayerParties.Include(x => x.Units.OrderBy(x => x.UnitNo))
            .Where(x => x.ViewerId == this.playerIdentityService.ViewerId);

    public IQueryable<DbPartyUnit> GetPartyUnits(IEnumerable<int> partySlots)
    {
        return apiContext
            .PlayerPartyUnits.Where(x =>
                x.ViewerId == this.playerIdentityService.ViewerId && partySlots.Contains(x.PartyNo)
            )
            .OrderBy(x => x.PartyNo == partySlots.First())
            .ThenBy(x => x.UnitNo);
    }

    public IQueryable<DbPartyUnit> GetPartyUnits(int firstParty, int secondParty)
    {
        return this.GetPartyUnits(new[] { firstParty, secondParty });
    }

    public IQueryable<DbPartyUnit> GetPartyUnits(int party)
    {
        return this.GetPartyUnits(new[] { party });
    }

    public async Task SetParty(DbParty newParty)
    {
        // TODO: this method executes a query where it deletes the old units and adds the new ones
        // Could it be optimized by updating the units instead? Anticipate that most changes will be small
        DbParty existingParty = await apiContext
            .PlayerParties.Where(x =>
                x.ViewerId == this.playerIdentityService.ViewerId && x.PartyNo == newParty.PartyNo
            )
            .Include(x => x.Units)
            .SingleAsync();

        existingParty.PartyName = newParty.PartyName;
        existingParty.Units = CleanUnitList(newParty.Units);

        apiContext.Entry(existingParty).State = EntityState.Modified;
    }

    public async Task UpdatePartyName(int partyNo, string newName)
    {
        DbParty existingParty = await apiContext
            .PlayerParties.Where(x =>
                x.ViewerId == this.playerIdentityService.ViewerId && x.PartyNo == partyNo
            )
            .Include(x => x.Units) // Need to return full unit list in update_data_list
            .SingleAsync();

        existingParty.PartyName = newName;
    }

    private static List<DbPartyUnit> CleanUnitList(ICollection<DbPartyUnit> original)
    {
        // For some reason, pressing 'Optimize' can send a request to /party/set_party_setting with like 8 units in it
        // Take the first one under each number, and fill in blanks if needed.
        List<DbPartyUnit> result = new();
        for (int i = 1; i <= 4; i++)
        {
            result.Add(
                original.FirstOrDefault(x => x.UnitNo == i)
                    ?? new()
                    {
                        ViewerId = original.First().ViewerId,
                        PartyNo = original.First().PartyNo,
                        UnitNo = i,
                        CharaId = 0
                    }
            );
        }

        return result;
    }
}
