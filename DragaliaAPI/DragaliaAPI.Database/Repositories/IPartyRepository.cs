using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IPartyRepository : IBaseRepository
{
    IQueryable<DbParty> Parties { get; }

    IQueryable<DbPartyUnit> GetPartyUnits(int firstParty, int secondParty);
    IQueryable<DbPartyUnit> GetPartyUnits(int party);
    IQueryable<DbPartyUnit> GetPartyUnits(IEnumerable<int> partySlots);
    Task SetParty(DbParty newParty);
    Task UpdatePartyName(int partyNo, string newName);
}
