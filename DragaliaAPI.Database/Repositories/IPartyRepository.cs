using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IPartyRepository : IBaseRepository
{
    IQueryable<DbParty> GetParties(string deviceAccountId);

    IQueryable<DbPartyUnit> GetPartyUnits(string deviceAccountId, int firstParty, int secondParty);
    IQueryable<DbPartyUnit> GetPartyUnits(string deviceAccountId, int party);
    IQueryable<DbPartyUnit> GetPartyUnits(string deviceAccountId, IEnumerable<int> partySlots);
    Task SetParty(string deviceAccountId, DbParty newParty);
    Task UpdatePartyName(string deviceAccountId, int partyNo, string newName);
}
