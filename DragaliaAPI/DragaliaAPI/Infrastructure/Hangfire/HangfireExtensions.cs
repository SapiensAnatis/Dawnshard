using Hangfire;
using Hangfire.Common;

namespace DragaliaAPI.Infrastructure.Hangfire;

public static class HangfireExtensions
{
    public static WebApplication AddHangfireJobs(this WebApplication app)
    {
        IRecurringJobManager jobClient = app.Services.GetRequiredService<IRecurringJobManager>();

        jobClient.AddOrUpdate(
            SummonHistoryJob.Id,
            Job.FromExpression<SummonHistoryJob>(s => s.PurgeSummonHistory()),
            Cron.Daily(01, 00)
        );

        jobClient.AddOrUpdate(
            HelperUseDateJob.Id,
            Job.FromExpression<HelperUseDateJob>(s => s.PurgeHelperUseDate()),
            Cron.Daily(06, 00) // daily reset
        );

        return app;
    }
}
