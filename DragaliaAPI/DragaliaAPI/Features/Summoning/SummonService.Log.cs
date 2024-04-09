using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Summoning;

public partial class SummonService
{
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Performing wyrmsigil trade {@Trade}")]
        public static partial void PerformingPointTrade(
            ILogger logger,
            AtgenSummonPointTradeList trade
        );

        [LoggerMessage(LogLevel.Information, "New wyrmsigil count: {NewPointAmount}")]
        public static partial void PointsDeducted(ILogger logger, int newPointAmount);

        [LoggerMessage(
            LogLevel.Warning,
            "Unexpected RewardService result for wyrmsigil trade: {Result}"
        )]
        public static partial void UnexpectedRewardResult(ILogger logger, RewardGrantResult result);
    }
}
