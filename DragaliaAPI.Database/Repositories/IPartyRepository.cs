using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IPartyRepository
{
    IQueryable<DbParty> GetParties(string deviceAccountId);
    Task SetParty(string deviceAccountId, DbParty newParty);
}
