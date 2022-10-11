using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

namespace DragaliaAPI.Services;

public interface ISavefileWriteService
{
    Task<UpdateDataList> CommitSummonResult(
        List<SummonEntity> summonResult,
        string deviceAccountId,
        bool giveDew = true
    );
}
