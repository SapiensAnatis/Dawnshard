using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestEnemyService
{
    IEnumerable<AtgenEnemy> BuildQuestEnemyList(int questId, int areaNum);

    IEnumerable<AtgenEnemy> BuildQuestWallEnemyList(int wallId, int wallLevel);
}
