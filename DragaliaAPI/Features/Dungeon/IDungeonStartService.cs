using DragaliaAPI.Features.Player;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonStartService
{
    Task<IngameData> GetIngameData(
        int questId,
        IList<int> partyNoList,
        ulong? supportViewerId = null
    );

    Task<IngameData> GetIngameData(
        int questId,
        IList<PartySettingList> party,
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
        IList<PartySettingList> party,
        ulong? supportViewerId = null
    );
}
