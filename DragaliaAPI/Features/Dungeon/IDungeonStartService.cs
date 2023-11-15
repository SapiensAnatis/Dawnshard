using DragaliaAPI.Features.Player;
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
    Task<bool> ValidateStamina(int questId, StaminaType staminaType);
    Task<IngameData> GetWallIngameData(
        int wallId,
        int wallLevel,
        int partyNo,
        ulong? supportViewerId = null
    );
    Task<IngameData> GetWallIngameData(
        int wallId,
        int wallLevel,
        IEnumerable<PartySettingList> party,
        ulong? supportViewerId = null
    );
}
