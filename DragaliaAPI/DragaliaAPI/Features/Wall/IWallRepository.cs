using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.Wall;

public interface IWallRepository
{
    IQueryable<DbPlayerQuestWall> QuestWalls { get; }

    Task AddInitialWall();

    Task<DbPlayerQuestWall> GetQuestWall(int wallId);
    Task<int> GetQuestWallLevel(QuestWallTypes type);
}
