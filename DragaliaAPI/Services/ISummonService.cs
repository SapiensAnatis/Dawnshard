using DragaliaAPI.Models.Dragalia.Responses.Common;

namespace DragaliaAPI.Services;

public interface ISummonService
{
    List<SummonEntity> GenerateSummonResult(int numSummons);
}
