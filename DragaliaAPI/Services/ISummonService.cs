using DragaliaAPI.Models.Dragalia.Responses.Common;

namespace DragaliaAPI.Services;

public interface ISummonService
{
    List<SimpleSummonReward> GenerateSummonResult(int numSummons);
}
