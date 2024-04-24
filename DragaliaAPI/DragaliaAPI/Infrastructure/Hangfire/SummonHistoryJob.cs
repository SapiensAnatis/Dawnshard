using DragaliaAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Infrastructure.Hangfire;

public partial class SummonHistoryJob(
    ApiContext apiContext,
    TimeProvider timeProvider,
    ILogger<SummonHistoryJob> logger
)
{
    public static string Id => "PurgeSummonHistory";

    public async Task PurgeSummonHistory()
    {
        DateTimeOffset cutOffTime = timeProvider.GetUtcNow() - TimeSpan.FromDays(14);

        Log.PurgingSummonHistory(logger, cutOffTime);

        await apiContext
            .PlayerSummonHistory.IgnoreQueryFilters()
            .Where(x => x.ExecDate < cutOffTime)
            .ExecuteDeleteAsync();
    }
}
