using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public interface IOddsInfoService
{
    OddsInfo GetOddsInfo(int questId, int areaNum);
    OddsInfo GetWallOddsInfo(int wallId, int wallLevel);
}
