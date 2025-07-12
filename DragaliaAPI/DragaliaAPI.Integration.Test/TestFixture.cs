using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.CoOp;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Login.Auth;
using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Shared.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using static DragaliaAPI.Infrastructure.DragaliaHttpConstants;

namespace DragaliaAPI.Integration.Test;

public class TestFixture
{
    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    protected string DeviceAccountId { get; } = $"logged_in_id_{Guid.NewGuid()}";

    /// <summary>
    /// The session ID which is associated with the logged in test user.
    /// </summary>
    protected string SessionId { get; } = $"session_id_{Guid.NewGuid()}";

    private readonly WebApplicationFactory<Program> factory;

    protected TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        this.TestOutputHelper = testOutputHelper;
        this.MockBaasApi = factory.MockBaasApi;
        this.MockPhotonStateApi = factory.MockPhotonStateApi;

        this.factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<TimeProvider>(this.MockTimeProvider);
            });
        });

        this.Client = this.CreateClient();

        this.Services = factory.Services.CreateScope().ServiceProvider;

        this.LastDailyReset = TimeProvider.System.GetLastDailyReset();

        this.SeedDatabase().Wait();
        this.SeedCache();

        IPlayerIdentityService stubPlayerIdentityService = new StubPlayerIdentityService(
            this.ViewerId
        );

        DbContextOptions<ApiContext> options = this.Services.GetRequiredService<
            DbContextOptions<ApiContext>
        >();
        this.ApiContext = new ApiContext(options, stubPlayerIdentityService);
        this.ApiContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        this.DungeonService = new DungeonService(
            this.Services.GetRequiredService<IDistributedCache>(),
            this.Services.GetRequiredService<IOptionsMonitor<RedisCachingOptions>>(),
            stubPlayerIdentityService,
            NullLogger<DungeonService>.Instance
        );
    }

    protected DateTimeOffset LastDailyReset { get; }

    protected Mock<IBaasApi> MockBaasApi { get; }

    protected Mock<IPhotonStateApi> MockPhotonStateApi { get; }

    protected FakeTimeProvider MockTimeProvider { get; } = new();

    protected ITestOutputHelper TestOutputHelper { get; }

    protected IServiceProvider Services { get; }

    /// <summary>
    /// The viewer ID associated with the logged in user.
    /// </summary>
    protected long ViewerId { get; private set; }

    protected HttpClient Client { get; set; }

    protected IDungeonService DungeonService { get; }

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
        {
            return;
        }

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
            if (entity.ViewerId == default)
            {
                entity.ViewerId = this.ViewerId;
            }

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

    protected long GetDragonKeyId(DragonId dragon)
    {
        return this
            .ApiContext.PlayerDragonData.Where(x => x.DragonId == dragon)
            .Select(x => x.DragonKeyId)
            .DefaultIfEmpty()
            .First();
    }

    protected HttpClient CreateClient(Action<IWebHostBuilder>? extraBuilderConfig = null)
    {
        WebApplicationFactory<Program> factoryToUse = this.factory;

        if (extraBuilderConfig is not null)
        {
            factoryToUse = this.factory.WithWebHostBuilder(extraBuilderConfig);
        }

        HttpClient client = factoryToUse.CreateClient(
            new WebApplicationFactoryClientOptions()
            {
                BaseAddress = new Uri("http://localhost/2.19.0_20220714193707/", UriKind.Absolute),
            }
        );

        client.DefaultRequestHeaders.Add(Headers.SessionId, this.SessionId);
        client.DefaultRequestHeaders.Add("Platform", "2");
        client.DefaultRequestHeaders.Add("Res-Ver", "y2XM6giU6zz56wCm");

        return client;
    }

    protected HttpClient CreateClientForOtherPlayer(
        DbPlayer player,
        Action<IWebHostBuilder>? extraBuilderConfig = null
    )
    {
        HttpClient client = this.CreateClient(extraBuilderConfig);

        string sessionId = $"session_id_other_player_{player.ViewerId}_{Guid.NewGuid()}";

        this.CreateSession(sessionId, player.AccountId, player.ViewerId);

        client.DefaultRequestHeaders.Remove(Headers.SessionId);
        client.DefaultRequestHeaders.Add(Headers.SessionId, sessionId);

        return client;
    }

    protected long GetTalismanKeyId(Talismans talisman)
    {
        return this
            .ApiContext.PlayerTalismans.Where(x =>
                x.ViewerId == this.ViewerId && x.TalismanId == talisman
            )
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
                    Quantity = 99999999,
                })
        );

        apiContext.PlayerDragonGifts.AddRange(
            Enum.GetValues<DragonGifts>()
                .Select(x => new DbPlayerDragonGift()
                {
                    ViewerId = newPlayer.ViewerId,
                    DragonGiftId = x,
                    Quantity = x < DragonGifts.FourLeafClover ? 1 : 999,
                })
        );

        IFortRepository fortRepository = this.Services.GetRequiredService<IFortRepository>();
        await fortRepository.InitializeFort();

        apiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                ViewerId = newPlayer.ViewerId,
                PlantId = FortPlants.Smithy,
                Level = 9,
            }
        );

        DbPlayerUserData userData = (
            await apiContext.PlayerUserData.FindAsync(newPlayer.ViewerId)
        )!;

        userData.Coin = 100_000_000;
        userData.Crystal = 1_200_000;
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
                Point2Quantity = 100_000_000,
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

    private void SeedCache() => CreateSession(this.SessionId, this.DeviceAccountId, this.ViewerId);

    private void CreateSession(string sessionId, string deviceAccountId, long viewerId)
    {
        IDistributedCache cache = this.Services.GetRequiredService<IDistributedCache>();

        Session session = new(
            sessionId,
            "id_token",
            deviceAccountId,
            viewerId,
            DateTimeOffset.MaxValue
        );
        cache.SetString($":session:session_id:{sessionId}", JsonSerializer.Serialize(session));
        cache.SetString($":session_id:device_account_id:{deviceAccountId}", sessionId);
    }
}
