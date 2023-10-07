using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Quest;

public interface IQuestService
{
    Task<int> GetQuestStamina(int questId, StaminaType type);
    Task<(
        DbQuest Quest,
        bool BestClearTime,
        IEnumerable<AtgenFirstClearSet> Bonus
    )> ProcessQuestCompletion(int questId, float clearTime, int playCount);

    Task<AtgenReceiveQuestBonus> ReceiveQuestBonus(int eventGroupId, bool isReceive, int count);
}
