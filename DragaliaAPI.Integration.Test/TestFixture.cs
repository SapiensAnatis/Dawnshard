﻿using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Helpers;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Integration.Test;

[Collection("DragaliaIntegration")]
public class TestFixture : IClassFixture<CustomWebApplicationFactory>
{
    protected TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        this.Client = factory
            .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddXUnit(outputHelper);
                    })
            )
            .CreateClient(
                new WebApplicationFactoryClientOptions()
                {
                    BaseAddress = new Uri("http://localhost/api/", UriKind.Absolute),
                }
            );

        this.Client.DefaultRequestHeaders.Add("SID", SessionId);
        this.Client.DefaultRequestHeaders.Add("Platform", "2");
        this.Client.DefaultRequestHeaders.Add("Res-Ver", "y2XM6giU6zz56wCm");

        this.Services = factory.Services.CreateScope().ServiceProvider;
        this.Mapper = factory.Services.GetRequiredService<IMapper>();
        this.ApiContext = factory.Services.GetRequiredService<ApiContext>();

        this.MockBaasApi = factory.MockBaasApi;
        this.MockPhotonStateApi = factory.MockPhotonStateApi;
        this.MockDateTimeProvider = factory.MockDateTimeProvider;

        this.MockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(() => DateTimeOffset.UtcNow);

        this.ViewerId = this.ApiContext.Players.First(x => x.AccountId == DeviceAccountId).ViewerId;
    }

    protected Mock<IBaasApi> MockBaasApi { get; }

    protected Mock<IPhotonStateApi> MockPhotonStateApi { get; }

    protected Mock<IDateTimeProvider> MockDateTimeProvider { get; }

    protected IServiceProvider Services { get; }

    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    public const string DeviceAccountId = "logged_in_id";

    protected long ViewerId { get; private set; }

    public const string SessionId = "session_id";

    protected HttpClient Client { get; }

    protected IMapper Mapper { get; }

    protected ApiContext ApiContext { get; }

    protected void AddCharacter(Charas id)
    {
        if (this.ApiContext.PlayerCharaData.Find(ViewerId, id) is not null)
            return;

        this.ApiContext.PlayerCharaData.Add(new(this.ViewerId, id));
        this.ApiContext.SaveChanges();
    }

    protected async Task<TEntity> AddToDatabase<TEntity>(TEntity data)
        where TEntity : class, IDbPlayerData
    {
        data.ViewerId = this.ViewerId;

        TEntity e = (await this.ApiContext.Set<TEntity>().AddAsync(data)).Entity;
        await this.ApiContext.SaveChangesAsync();

        return e;
    }

    protected async Task AddToDatabase<TEntity>(params TEntity[] data)
        where TEntity : class, IDbPlayerData
    {
        foreach (TEntity entity in data)
            entity.ViewerId = this.ViewerId;

        await this.ApiContext.Set<TEntity>().AddRangeAsync(data);
        await this.ApiContext.SaveChangesAsync();
    }

    protected async Task AddRangeToDatabase<TEntity>(IEnumerable<TEntity> data)
        where TEntity : class, IDbPlayerData
    {
        foreach (TEntity entity in data)
            entity.ViewerId = this.ViewerId;

        await this.ApiContext.AddRangeAsync((IEnumerable<object>)data);
        await this.ApiContext.SaveChangesAsync();
    }

    protected void SetupSaveImport()
    {
        this.MockBaasApi.Setup(x => x.GetSavefile(It.IsAny<string>())).ReturnsAsync(GetSavefile());
    }

    protected void ImportSave()
    {
        if (
            this.ApiContext
                .PlayerUserData
                .AsNoTracking()
                .First(x => x.ViewerId == ViewerId)
                .LastSaveImportTime > DateTimeOffset.UnixEpoch
        )
        {
            return;
        }

        ISavefileService savefileService = this.Services.GetRequiredService<ISavefileService>();
        IPlayerIdentityService playerIdentityService =
            this.Services.GetRequiredService<IPlayerIdentityService>();

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            ViewerId,
            DeviceAccountId
        );

        savefileService.Import(GetSavefile()).Wait();
    }

    protected long GetDragonKeyId(Dragons dragon)
    {
        return this.ApiContext
            .PlayerDragonData
            .Where(x => x.DragonId == dragon)
            .Select(x => x.DragonKeyId)
            .DefaultIfEmpty()
            .First();
    }

    protected long GetTalismanKeyId(Talismans talisman)
    {
        return this.ApiContext
            .PlayerTalismans
            .Where(x => x.TalismanId == talisman)
            .Select(x => x.TalismanKeyId)
            .DefaultIfEmpty()
            .First();
    }

    private static LoadIndexData GetSavefile() =>
        JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexData>>(
                File.ReadAllText(Path.Join("Data", "endgame_savefile.json")),
                ApiJsonOptions.Instance
            )!
            .data;
}
