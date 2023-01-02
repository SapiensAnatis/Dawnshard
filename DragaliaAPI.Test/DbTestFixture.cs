using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test;

public class DbTestFixture : IDisposable
{
    public ApiContext ApiContext { get; init; }

    public const string DeviceAccountId = "id";

    public DbTestFixture()
    {
        DbContextOptions<ApiContext> options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase($"DbTestFixture-{Guid.NewGuid()}")
            .Options;

        this.ApiContext = new ApiContext(options);
        Mock<ILogger<SavefileService>> mockLogger = new(MockBehavior.Loose);

        ISavefileService deviceAccountRepository = new SavefileService(
            this.ApiContext,
            new MapperConfiguration(opts => opts.AddMaps(typeof(Program).Assembly)).CreateMapper(),
            mockLogger.Object
        );
        deviceAccountRepository.CreateNewSavefile("id").Wait();
    }

    public async Task AddToDatabase<TEntity>(TEntity data)
    {
        if (data is null)
            return;

        await this.ApiContext.AddAsync(data);
        await this.ApiContext.SaveChangesAsync();
    }

    public async Task AddRangeToDatabase<TEntity>(IEnumerable<TEntity> data)
    {
        if (data is null)
            return;

        await this.ApiContext.AddRangeAsync((IEnumerable<object>)data);
        await this.ApiContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        this.ApiContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
