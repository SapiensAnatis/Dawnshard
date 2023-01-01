﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test;

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

        IDeviceAccountRepository deviceAccountRepository = new DeviceAccountRepository(
            this.ApiContext
        );
        deviceAccountRepository.CreateNewSavefile("id").Wait();
        deviceAccountRepository.SaveChangesAsync().Wait();
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
