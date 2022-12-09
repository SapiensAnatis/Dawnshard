using DragaliaAPI.Models;

namespace DragaliaAPI.Services;

public interface IDungeonService
{
    Task<DungeonSession> FinishDungeon(string dungeonKey);
    Task<DungeonSession> GetDungeon(string dungeonKey);
    Task<string> StartDungeon(DungeonSession dungeonSession);
}
