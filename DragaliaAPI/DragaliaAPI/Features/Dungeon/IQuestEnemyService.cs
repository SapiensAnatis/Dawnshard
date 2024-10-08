using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestEnemyService
{
    IList<AtgenEnemy> BuildQuestEnemyList(int questId, int areaNum);

    IList<AtgenEnemy> BuildQuestWallEnemyList(int wallId, int wallLevel);
}
