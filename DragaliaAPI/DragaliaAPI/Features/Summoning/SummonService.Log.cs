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

        [LoggerMessage(LogLevel.Warning, "Failed to find banner {BannerId}")]
        public static partial void BannerNotFound(ILogger logger, int bannerId);

        [LoggerMessage(
            LogLevel.Warning,
            "Failed to find summon trade {TradeId} for banner {BannerId}"
        )]
        public static partial void TradeNotFound(ILogger logger, int tradeId, int bannerId);

        [LoggerMessage(
            LogLevel.Warning,
            "Failed to perform summon trade {TradeId}: point quantity {SummonPoints} is insufficient"
        )]
        public static partial void NotEnoughPointsForTrade(
            ILogger logger,
            int tradeId,
            int summonPoints
        );

        [LoggerMessage(
            LogLevel.Warning,
            "Unexpected RewardService result for wyrmsigil trade: {Result}"
        )]
        public static partial void UnexpectedRewardResult(ILogger logger, RewardGrantResult result);
    }
}
