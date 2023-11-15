using DragaliaAPI.Models;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonService
{
    Task<DungeonSession> FinishDungeon(string dungeonKey);
    Task<DungeonSession> GetDungeon(string dungeonKey);
    Task ModifySession(string dungeonKey, Action<DungeonSession> update);
    Task<string> StartDungeon(DungeonSession dungeonSession);
}
