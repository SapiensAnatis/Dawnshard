﻿using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace DragaliaAPI.Database.Test;

public class DbTestFixture : IDisposable
{
    public ApiContext ApiContext { get; init; }

    public const string DeviceAccountId = "id";

    public DbTestFixture()
    {
        DbContextOptions<ApiContext> options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase($"DbTestFixture-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        this.ApiContext = new ApiContext(options);
        Mock<ILogger<SavefileService>> mockLogger = new(MockBehavior.Loose);
        // Unused for creating saves
        Mock<IDistributedCache> mockCache = new(MockBehavior.Loose);

        SavefileService savefileService =
            new(
                this.ApiContext,
                mockCache.Object,
                new MapperConfiguration(
                    opts => opts.AddMaps(typeof(Program).Assembly)
                ).CreateMapper(),
                mockLogger.Object,
                IdentityTestUtils.MockPlayerDetailsService.Object,
                Enumerable.Empty<ISavefileUpdate>()
            );
        savefileService.Create().Wait();
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
