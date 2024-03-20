using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

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
        // Used but we probably don't want it to actually add characters?
        Mock<IUnitRepository> mockUnitRepository = new(MockBehavior.Loose);

        SavefileService savefileService =
            new(
                this.ApiContext,
                mockCache.Object,
                new MapperConfiguration(opts =>
                    opts.AddMaps(typeof(Program).Assembly)
                ).CreateMapper(),
                mockLogger.Object,
                IdentityTestUtils.MockPlayerDetailsService.Object,
                Enumerable.Empty<ISavefileUpdate>(),
                mockUnitRepository.Object
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
