using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Summoning;

public interface ISummonService
{
    List<AtgenResultUnitList> GenerateRewardList(
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    );

    List<AtgenRedoableSummonResultUnitList> GenerateSummonResult(int numSummons);
}
