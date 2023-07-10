using DragaliaAPI.Models;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonService
{
    Task<DungeonSession> FinishDungeon(string dungeonKey);
    Task<DungeonSession> GetDungeon(string dungeonKey);
    Task<string> StartDungeon(DungeonSession dungeonSession);
}
