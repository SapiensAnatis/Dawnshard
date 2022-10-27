using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

namespace DragaliaAPI.Services;

public interface ISavefileWriteService
{
    Task<UpdateDataList> CommitSummonResult(
        List<SimpleSummonReward> summonResult,
        string deviceAccountId,
        bool giveDew = true
    );

    Task<int> CreateSummonHistory(IEnumerable<DbPlayerSummonHistory> rewards);
}
