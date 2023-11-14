using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Wall;

public interface IWallRepository
{
    IQueryable<DbPlayerQuestWall> QuestWalls { get; }

    Task InitializeWall();

    Task<DbPlayerQuestWall> GetQuestWall(int wallId);
}
