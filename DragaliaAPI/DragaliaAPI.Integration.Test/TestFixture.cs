using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Npgsql;

namespace DragaliaAPI.Integration.Test;

[Collection(TestCollection.Name)]
public class TestFixture
{
    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    protected const string DeviceAccountId = "logged_in_id";

    /// <summary>
    /// The session ID which is associated with the logged in test user.
    /// </summary>
    protected const string SessionId = "session_id";

    private readonly CustomWebApplicationFactory factory;

    protected TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        this.factory = factory;
        this.TestOutputHelper = testOutputHelper;

        this.Client = this.CreateClient();

        this.MockBaasApi.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);

        this.Services = factory.Services.CreateScope().ServiceProvider;

        this.Mapper = this.Services.GetRequiredService<IMapper>();
        this.LastDailyReset = this.Services.GetRequiredService<IResetHelper>().LastDailyReset;

        this.SeedDatabase().Wait();
        this.SeedCache().Wait();

        DbContextOptions<ApiContext> options = this.Services.GetRequiredService<
            DbContextOptions<ApiContext>
        >();
        this.ApiContext = new ApiContext(options, new StubPlayerIdentityService(this.ViewerId));
        this.ApiContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected DateTimeOffset LastDailyReset { get; }

    protected Mock<IBaasApi> MockBaasApi => this.factory.MockBaasApi;

    protected Mock<IPhotonStateApi> MockPhotonStateApi => this.factory.MockPhotonStateApi;

    protected FakeTimeProvider MockTimeProvider { get; } = new();

    protected ITestOutputHelper TestOutputHelper { get; }

    protected IServiceProvider Services { get; }

    /// <summary>
    /// The viewer ID associated with the logged in user.
    /// </summary>
    /// <remarks>
    /// This is not a constant -- although the database is cleared in <see cref="SeedDatabase"/> between each test,
    /// the seeding of the identity column is not reset, so each test increments the viewer ID by 1.
    /// </remarks>
    protected long ViewerId { get; private set; }

    protected HttpClient Client { get; set; }

    protected IMapper Mapper { get; }

    /// <summary>
    /// Instance of <see cref="ApiContext"/> to use for setting up / interrogating the database in tests.
    /// </summary>
    /// <remarks>
    /// This has the change tracking behaviour set to <see cref="QueryTrackingBehavior.NoTracking"/> (i.e. change
    /// tracking is disabled). If you want to modify existing data in a test, use
    /// <see cref="EntityFrameworkQueryableExtensions.AsTracking{TEntity}(IQueryable{TEntity})"/> on the query.
    /// </remarks>
    protected ApiContext ApiContext { get; }

    protected void AddCharacter(Charas id)
    {
        if (this.ApiContext.PlayerCharaData.Find(ViewerId, id) is not null)
            return;

        this.ApiContext.PlayerCharaData.Add(new(this.ViewerId, id));
        this.ApiContext.SaveChanges();
    }

    protected async Task<TEntity> AddToDatabase<TEntity>(TEntity data)
        where TEntity : IDbPlayerData
    {
        data.ViewerId = this.ViewerId;
        this.ApiContext.Add(data);

        await this.ApiContext.SaveChangesAsync();
        this.ApiContext.ChangeTracker.Clear();

        return data;
    }

    protected Task AddToDatabase(params IDbPlayerData[] data) => this.AddRangeToDatabase(data);

    protected async Task AddRangeToDatabase(IEnumerable<IDbPlayerData> data)
    {
        foreach (IDbPlayerData entity in data)
        {
            entity.ViewerId = this.ViewerId;
            this.ApiContext.Add(entity);
        }

        await this.ApiContext.SaveChangesAsync();
    }

    protected void SetupSaveImport()
    {
        this.MockBaasApi.Setup(x => x.GetSavefile(It.IsAny<string>())).ReturnsAsync(GetSavefile());
    }

    protected async Task ImportSave()
    {
        using IServiceScope scope = this.factory.Services.CreateScope();

        ISavefileService savefileService =
            scope.ServiceProvider.GetRequiredService<ISavefileService>();
        IPlayerIdentityService playerIdentityService =
            scope.ServiceProvider.GetRequiredService<IPlayerIdentityService>();

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            ViewerId,
            DeviceAccountId
        );

        await savefileService.Import(GetSavefile());
    }

    protected long GetDragonKeyId(Dragons dragon)
    {
        return this
            .ApiContext.PlayerDragonData.Where(x => x.DragonId == dragon)
            .Select(x => x.DragonKeyId)
            .DefaultIfEmpty()
            .First();
    }

    protected HttpClient CreateClient(Action<IWebHostBuilder>? extraBuilderConfig = null)
    {
        HttpClient client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddXUnit(this.TestOutputHelper);
                });
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<TimeProvider>(this.MockTimeProvider);
                });
                extraBuilderConfig?.Invoke(builder);
            })
            .CreateClient(
                new WebApplicationFactoryClientOptions()
                {
                    BaseAddress = new Uri("http://localhost/api/", UriKind.Absolute),
                }
            );

        client.DefaultRequestHeaders.Add("SID", SessionId);
        client.DefaultRequestHeaders.Add("Platform", "2");
        client.DefaultRequestHeaders.Add("Res-Ver", "y2XM6giU6zz56wCm");

        return client;
    }

    protected long GetTalismanKeyId(Talismans talisman)
    {
        return this
            .ApiContext.PlayerTalismans.Where(x => x.TalismanId == talisman)
            .Select(x => x.TalismanKeyId)
            .DefaultIfEmpty()
            .First();
    }

    private static LoadIndexResponse GetSavefile() =>
        JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexResponse>>(
                File.ReadAllText(Path.Join("Data", "endgame_savefile.json")),
                ApiJsonOptions.Instance
            )!
            .Data;

    private async Task SeedDatabase()
    {
        await using NpgsqlConnection connection = new(this.factory.PostgresConnectionString);
        await connection.OpenAsync();

        ArgumentNullException.ThrowIfNull(this.factory.Respawner);
        await this.factory.Respawner.ResetAsync(connection);

        ISavefileService savefileService = this.Services.GetRequiredService<ISavefileService>();
        IPlayerIdentityService playerIdentityService =
            this.Services.GetRequiredService<IPlayerIdentityService>();
        ApiContext apiContext = this.Services.GetRequiredService<ApiContext>();

        DbPlayer newPlayer = await savefileService.Create(DeviceAccountId);

        this.ViewerId = newPlayer.ViewerId;

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            newPlayer.ViewerId,
            newPlayer.AccountId
        );

        apiContext.PlayerMaterials.AddRange(
            Enum.GetValues<Materials>()
                .Select(x => new DbPlayerMaterial()
                {
                    ViewerId = newPlayer.ViewerId,
                    MaterialId = x,
                    Quantity = 99999999
                })
        );

        apiContext.PlayerDragonGifts.AddRange(
            Enum.GetValues<DragonGifts>()
                .Select(x => new DbPlayerDragonGift()
                {
                    ViewerId = newPlayer.ViewerId,
                    DragonGiftId = x,
                    Quantity = x < DragonGifts.FourLeafClover ? 1 : 999
                })
        );

        IFortRepository fortRepository = this.Services.GetRequiredService<IFortRepository>();
        await fortRepository.InitializeFort();

        apiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                ViewerId = newPlayer.ViewerId,
                PlantId = FortPlants.Smithy,
                Level = 9
            }
        );

        DbPlayerUserData userData = (
            await apiContext.PlayerUserData.FindAsync(newPlayer.ViewerId)
        )!;

        userData.Coin = 100_000_000;
        userData.DewPoint = 100_000_000;
        userData.ManaPoint = 100_000_000;
        userData.Level = 250;
        userData.Exp = 28253490;
        userData.StaminaSingle = 999;
        userData.QuestSkipPoint = 300;

        apiContext.PlayerDmodeInfos.Add(
            new DbPlayerDmodeInfo
            {
                ViewerId = newPlayer.ViewerId,
                Point1Quantity = 100_000_000,
                Point2Quantity = 100_000_000
            }
        );

        apiContext.PlayerDmodeDungeons.Add(
            new DbPlayerDmodeDungeon { ViewerId = newPlayer.ViewerId }
        );

        apiContext.PlayerDmodeExpeditions.Add(
            new DbPlayerDmodeExpedition { ViewerId = newPlayer.ViewerId }
        );

        await apiContext.SaveChangesAsync();
        apiContext.ChangeTracker.Clear();
    }

    private async Task SeedCache()
    {
        this.factory.ResetCache();

        IDistributedCache cache = this.Services.GetRequiredService<IDistributedCache>();

        Session session =
            new(SessionId, "id_token", DeviceAccountId, this.ViewerId, DateTimeOffset.MaxValue);
        await cache.SetStringAsync(
            ":session:session_id:session_id",
            JsonSerializer.Serialize(session)
        );
        await cache.SetStringAsync(":session_id:device_account_id:logged_in_id", SessionId);
    }
}
