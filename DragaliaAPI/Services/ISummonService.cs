using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISummonService
{
    List<AtgenResultUnitList> GenerateRewardList(
        string deviceAccountId,
        IEnumerable<SimpleSummonReward> baseRewardList
    );

    List<SimpleSummonReward> GenerateSummonResult(int numSummons);
}
