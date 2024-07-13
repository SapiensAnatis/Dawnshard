using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Summon;

namespace DragaliaAPI.Features.Summoning;

public readonly record struct SummonRequestInfo(
    int ExecCount,
    int SummonCount,
    int ResultSummonPoint,
    SummonRequestRequest Request
)
{
    public static SummonRequestInfo FromSummonRequest(
        SummonRequestRequest request,
        SummonList summonList
    )
    {
        int execCount = request.ExecCount > 0 ? request.ExecCount : 1;
        int summonCount = request.ExecType == SummonExecTypes.Tenfold ? 10 : execCount;

        int addSummonPoint =
            request.PaymentType == PaymentTypes.Diamantium
                ? summonList.AddSummonPointStone
                : summonList.AddSummonPoint;

        return new(
            ExecCount: execCount,
            SummonCount: summonCount,
            ResultSummonPoint: addSummonPoint * summonCount,
            Request: request
        );
    }

    public PaymentTarget PaymentTarget => this.Request.PaymentTarget;

    public PaymentTypes PaymentType => this.Request.PaymentType;

    public int SummonId => this.Request.SummonId;

    public SummonExecTypes ExecType => this.Request.ExecType;
}
