using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure.Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Other;

public class SummonHistoryJobTest : TestFixture
{
    private readonly SummonHistoryJob summonHistoryJob;

    public SummonHistoryJobTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.summonHistoryJob = ActivatorUtilities.CreateInstance<SummonHistoryJob>(this.Services);
    }

    [Fact]
    public async Task SummonHistoryJob_PurgesOlderThan14Days()
    {
        await this.AddRangeToDatabase(
            [
                new DbPlayerSummonHistory() { ExecDate = DateTimeOffset.UtcNow },
                new DbPlayerSummonHistory() { ExecDate = DateTimeOffset.UtcNow.AddDays(-20) }
            ]
        );

        await this.summonHistoryJob.PurgeSummonHistory();

        this.ApiContext.PlayerSummonHistory.Should().ContainSingle();
    }
}
