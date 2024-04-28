using DragaliaAPI.DTO;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Player;

namespace DragaliaAPI.Features.Quest;

public interface IQuestService
{
    Task<int> GetQuestStamina(int questId, StaminaType type);
    Task<(bool BestClearTime, IEnumerable<AtgenFirstClearSet> Bonus)> ProcessQuestCompletion(
        DungeonSession session,
        PlayRecord playRecord
    );

    Task<AtgenReceiveQuestBonus> ReceiveQuestBonus(int eventGroupId, bool isReceive, int count);
}
