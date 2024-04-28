using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Dungeon;

public interface IOddsInfoService
{
    OddsInfo GetOddsInfo(int questId, int areaNum);
    OddsInfo GetWallOddsInfo(int wallId, int wallLevel);
}
