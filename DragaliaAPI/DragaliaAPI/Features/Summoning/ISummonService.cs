using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Summoning;

public interface ISummonService
{
    Task<List<AtgenResultUnitList>> GenerateRewardList(
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    );

    Task<List<AtgenRedoableSummonResultUnitList>> GenerateSummonResult(
        int numSummons,
        int bannerId,
        SummonExecTypes execType
    );

    Task<IEnumerable<SummonTicketList>> GetSummonTicketList();
    Task<List<AtgenRedoableSummonResultUnitList>> GenerateRedoableSummonResult();
}
