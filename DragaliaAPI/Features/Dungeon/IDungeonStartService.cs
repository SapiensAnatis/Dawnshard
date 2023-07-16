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
}
