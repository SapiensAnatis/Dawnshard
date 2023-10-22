using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestDropService
{
    (IEnumerable<Materials>, IEnumerable<AbilityCrests>) GetDrops(int questId);
}
