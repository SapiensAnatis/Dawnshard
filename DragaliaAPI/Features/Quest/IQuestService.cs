using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Quest;

public interface IQuestService
{
    Task<(
        DbQuest Quest,
        bool BestClearTime,
        IEnumerable<AtgenFirstClearSet> Bonus
    )> ProcessQuestCompletion(int questId, float clearTime, int playCount);

    Task<AtgenReceiveQuestBonus> ReceiveQuestBonus(int eventGroupId, bool isReceive, int count);
}
