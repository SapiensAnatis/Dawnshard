﻿using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Infrastructure.Metrics;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DragaliaAPI.Database.Test;

public class DbTestFixture : IDisposable
{
    public ApiContext ApiContext { get; init; }

    public const long ViewerId = 1;

    public DbTestFixture()
    {
        DbContextOptions<ApiContext> options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase($"DbTestFixture-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        this.ApiContext = new ApiContext(options, new StubPlayerIdentityService(ViewerId));
        // Unused for creating saves
        Mock<ILogger<SavefileService>> mockLogger = new(MockBehavior.Loose);
        Mock<IDistributedCache> mockCache = new(MockBehavior.Loose);

        SavefileService savefileService = new(
            this.ApiContext,
            mockCache.Object,
            mockLogger.Object,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            [],
            Substitute.For<IDragaliaApiMetrics>()
        );
        savefileService.Create().Wait();
    }

    public async Task AddToDatabase<TEntity>(TEntity data)
    {
        if (data is null)
            return;

        await this.ApiContext.AddAsync(data);
        await this.ApiContext.SaveChangesAsync();
        this.ApiContext.ChangeTracker.Clear();
    }

    public async Task AddRangeToDatabase<TEntity>(IEnumerable<TEntity> data)
    {
        await this.ApiContext.AddRangeAsync((IEnumerable<object>)data);
        await this.ApiContext.SaveChangesAsync();
        this.ApiContext.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        this.ApiContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
