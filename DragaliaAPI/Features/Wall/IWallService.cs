using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Wall;

public interface IWallService
{
    Task LevelupQuestWall(int wallId);

    Task<IEnumerable<QuestWallList>> GetQuestWallList();

    Task<int> GetTotalWallLevel();

}
