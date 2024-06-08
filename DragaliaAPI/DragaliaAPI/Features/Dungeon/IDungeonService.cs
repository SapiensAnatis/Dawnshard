using DragaliaAPI.Models;

namespace DragaliaAPI.Features.Dungeon;

public interface IDungeonService
{
    Task RemoveSession(string dungeonKey, CancellationToken cancellationToken);
    Task<DungeonSession> GetSession(string dungeonKey, CancellationToken cancellationToken);
    Task ModifySession(
        string dungeonKey,
        Action<DungeonSession> update,
        CancellationToken cancellationToken
    );
    string CreateSession(DungeonSession dungeonSession);
    Task SaveSession(CancellationToken cancellationToken);
}
