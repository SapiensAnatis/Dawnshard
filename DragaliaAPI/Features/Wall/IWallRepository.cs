using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Wall;

public interface IWallRepository
{
    IQueryable<DbPlayerQuestWall> QuestWalls { get; }

    Task InitializeWall();

    Task<DbPlayerQuestWall> GetQuestWall(int wallId);
}
