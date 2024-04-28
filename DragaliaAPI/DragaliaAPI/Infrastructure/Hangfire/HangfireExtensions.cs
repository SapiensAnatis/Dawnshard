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

        return app;
    }
}
