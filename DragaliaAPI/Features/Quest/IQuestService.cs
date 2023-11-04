using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Quest;

public interface IQuestService
{
    Task<int> GetQuestStamina(int questId, StaminaType type);
    Task<(
        DbQuest Quest,
        bool BestClearTime,
        IEnumerable<AtgenFirstClearSet> Bonus
    )> ProcessQuestCompletion(DungeonSession session, float clearTime);

    Task<AtgenReceiveQuestBonus> ReceiveQuestBonus(int eventGroupId, bool isReceive, int count);
}
