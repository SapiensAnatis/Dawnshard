using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISummonService
{
    List<AtgenResultUnitList> GenerateRewardList(
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    );

    List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(int numSummons);
}
