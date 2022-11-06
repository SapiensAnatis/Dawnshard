using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Services;

public interface ISummonService
{
    IEnumerable<SummonReward> GenerateRewardList(
        IEnumerable<SimpleSummonReward> baseRewardList,
        IEnumerable<DbPlayerCharaData> repositoryCharaOutput,
        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) repositoryDragonOutput,
        bool giveDewPoint = true
    );
    List<SimpleSummonReward> GenerateSummonResult(int numSummons);
    UpdateDataList GenerateUpdateData(
        IEnumerable<DbPlayerCharaData> repositoryCharaOutput,
        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliability
        ) repositoryDragonOutput
    );
}
