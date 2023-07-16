using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonRepository
{
    IEnumerable<IQueryable<DbDetailedPartyUnit>> BuildDetailedPartyUnit(
        IEnumerable<PartySettingList> input
    );

    IQueryable<DbDetailedPartyUnit> BuildDetailedPartyUnit(
        IQueryable<DbPartyUnit> input,
        int firstPartyNo
    );

    IQueryable<DbDetailedPartyUnit> BuildDetailedPartyUnit(IQueryable<DbQuestClearPartyUnit> input);
}
