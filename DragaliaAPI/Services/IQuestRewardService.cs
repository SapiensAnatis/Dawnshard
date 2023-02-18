using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IQuestRewardService
{
    IEnumerable<Materials> GetDrops(int questId);
}
