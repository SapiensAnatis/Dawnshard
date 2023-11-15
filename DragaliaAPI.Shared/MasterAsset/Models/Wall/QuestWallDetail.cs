using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Wall;

public record QuestWallDetail(
    int Id,
    int WallId,
    int WallLevel,
    UnitElement LimitedElementalType,
    int Difficulty,
    int ClearTermsType,
    int FailedTermsType,
    double FailedTermsTimeElapsed,
    DungeonTypes DungeonType,
    string Scene01,
    string AreaName01,
    string WallBgm,
    int BossEnemyParamId,
    int WallAbnormalStatusType1,
    int WallAbnormalStatusType2,
    int WallAbnormalStatusType3,
    int DropEntityType1,
    int DropEntityId1,
    int DropEntityQuantity1,
    int DropEntityType2,
    int DropEntityId2,
    int DropEntityQuantity2,
    int DropEntityType3,
    int DropEntityId3,
    int DropEntityQuantity3,
    string ViewStartDate,
    string ViewEndDate,
    int ClearCoin,
    int ClearMana,
    int ClearEntityType1,
    int ClearEntityId1,
    int ClearEntityQuantity1,
    int ClearEntityType2,
    int ClearEntityId2,
    int ClearEntityQuantity2,
    int ClearEntityType3,
    int ClearEntityId3,
    int ClearEntityQuantity3,
    int ClearEntityType4,
    int ClearEntityId4,
    int ClearEntityQuantity4,
    int ClearEntityType5,
    int ClearEntityId5,
    int ClearEntityQuantity5
)
{
    public IEnumerable<AreaInfo> AreaInfo =>
        new List<AreaInfo>() { new(Scene01, AreaName01) }.Where(
            x => !string.IsNullOrEmpty(x.ScenePath) && !string.IsNullOrEmpty(x.AreaName)
        );
}
