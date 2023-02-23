using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestEnemyService
{
    IEnumerable<AtgenEnemy> BuildEnemyList(int questId, int areaNum);
}
