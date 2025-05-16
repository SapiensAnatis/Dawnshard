using DragaliaAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Infrastructure.Hangfire;

/// <summary>
/// Purge records of helper use dates at reset.
/// </summary>
/// <remarks>
/// Any record of a helper being used prior to reset is ignored, so the rows at that point serve no purpose.
/// </remarks>
public class HelperUseDateJob(ApiContext apiContext, TimeProvider timeProvider)
{
    public static string Id => "PurgeHelperUseDate";

    public async Task PurgeHelperUseDate()
    {
        await apiContext
            .PlayerHelperUseDates.IgnoreQueryFilters()
            .Where(x => x.UseDate <= timeProvider.GetLastDailyReset())
            .ExecuteDeleteAsync();
    }
}
