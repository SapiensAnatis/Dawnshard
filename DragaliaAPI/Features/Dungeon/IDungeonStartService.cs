using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonStartService
{
    Task<IngameData> GetIngameData(
        int questId,
        IEnumerable<int> partyNoList,
        ulong? supportViewerId = null
    );

    Task<IngameData> GetIngameData(
        int questId,
        IEnumerable<PartySettingList> party,
        ulong? supportViewerId = null
    );

    Task<IngameQuestData> InitiateQuest(int questId);
    Task<IngameData> GetWallIngameData(WallStartStartAssignUnitRequest request);
    Task<IngameData> GetWallIngameData(WallStartStartRequest request);
}
