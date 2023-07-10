using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonService
{
    Task AddEnemies(string dungeonKey, int areaIndex, IEnumerable<AtgenEnemy> enemyList);
    Task<DungeonSession> FinishDungeon(string dungeonKey);
    Task<DungeonSession> GetDungeon(string dungeonKey);
    Task<string> StartDungeon(DungeonSession dungeonSession);
}
