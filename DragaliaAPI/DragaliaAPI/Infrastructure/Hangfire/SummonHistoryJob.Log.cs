namespace DragaliaAPI.Infrastructure.Hangfire;

public partial class SummonHistoryJob
{
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Purging all summon history older than {CutOffDate}")]
        public static partial void PurgingSummonHistory(ILogger logger, DateTimeOffset cutOffDate);
    }
}
