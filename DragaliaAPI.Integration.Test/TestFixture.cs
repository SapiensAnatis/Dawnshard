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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;

namespace DragaliaAPI.Integration.Test;

[Collection("DragaliaIntegration")]
public class TestFixture : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    protected const string DeviceAccountId = "logged_in_id";

    /// <summary>
    /// The session ID which is associated with the logged in test user.
    /// </summary>
    private const string SessionId = "session_id";

    private readonly CustomWebApplicationFactory factory;

    protected TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        this.factory = factory;
        this.TestOutputHelper = testOutputHelper;

        this.Client = factory
            .WithWebHostBuilder(
                builder =>
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddXUnit(this.TestOutputHelper);
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

        this.MockBaasApi.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);
        this.MockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(() => DateTimeOffset.UtcNow);

        this.Services = factory.Services.CreateScope().ServiceProvider;

        this.Mapper = this.Services.GetRequiredService<IMapper>();
        this.ApiContext = this.Services.GetRequiredService<ApiContext>();
        this.LastDailyReset = this.Services.GetRequiredService<IResetHelper>().LastDailyReset;

        this.ApiContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected DateTimeOffset LastDailyReset { get; }

    protected Mock<IBaasApi> MockBaasApi => this.factory.MockBaasApi;

    protected Mock<IPhotonStateApi> MockPhotonStateApi => this.factory.MockPhotonStateApi;

    protected Mock<IDateTimeProvider> MockDateTimeProvider => this.factory.MockDateTimeProvider;

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

    protected HttpClient Client { get; }

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

    public async Task InitializeAsync()
    {
        await this.SeedDatabase();
        await this.SeedCache();
        await this.Setup();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Defines test-specific setup to run at the end of <see cref="InitializeAsync"/>.
    /// </summary>
    /// <remarks>
    /// Override this and put test-specific database seeding here, and not in a constructor, as the initial database
    /// seeding (including the creation of the test savefile) is done in <see cref="InitializeAsync"/> which runs
    /// after the constructor(s).
    /// </remarks>
    /// <returns>A task that performs setup operations.</returns>
    protected virtual Task Setup() => Task.CompletedTask;

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
        return this.ApiContext.PlayerDragonData.Where(x => x.DragonId == dragon)
            .Select(x => x.DragonKeyId)
            .DefaultIfEmpty()
            .First();
    }

    protected long GetTalismanKeyId(Talismans talisman)
    {
        return this.ApiContext.PlayerTalismans.Where(x => x.TalismanId == talisman)
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

    private async Task SeedDatabase()
    {
        await using NpgsqlConnection connection = new(this.factory.PostgresConnectionString);
        await connection.OpenAsync();

        ArgumentNullException.ThrowIfNull(this.factory.Respawner);
        await this.factory.Respawner.ResetAsync(connection);

        ISavefileService savefileService = this.Services.GetRequiredService<ISavefileService>();
        IPlayerIdentityService playerIdentityService =
            this.Services.GetRequiredService<IPlayerIdentityService>();

        DbPlayer newPlayer = await savefileService.Create(DeviceAccountId);

        this.ViewerId = newPlayer.ViewerId;

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            newPlayer.ViewerId,
            newPlayer.AccountId
        );

        this.ApiContext.PlayerMaterials.AddRange(
            Enum.GetValues<Materials>()
                .Select(
                    x =>
                        new DbPlayerMaterial()
                        {
                            ViewerId = newPlayer.ViewerId,
                            MaterialId = x,
                            Quantity = 99999999
                        }
                )
        );

        this.ApiContext.PlayerDragonGifts.AddRange(
            Enum.GetValues<DragonGifts>()
                .Select(
                    x =>
                        new DbPlayerDragonGift()
                        {
                            ViewerId = newPlayer.ViewerId,
                            DragonGiftId = x,
                            Quantity = x < DragonGifts.FourLeafClover ? 1 : 999
                        }
                )
        );

        IFortRepository fortRepository = this.Services.GetRequiredService<IFortRepository>();
        await fortRepository.InitializeFort();

        this.ApiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                ViewerId = newPlayer.ViewerId,
                PlantId = FortPlants.Smithy,
                Level = 9
            }
        );

        DbPlayerUserData userData = (
            await this.ApiContext.PlayerUserData.FindAsync(newPlayer.ViewerId)
        )!;

        userData.Coin = 100_000_000;
        userData.DewPoint = 100_000_000;
        userData.ManaPoint = 100_000_000;
        userData.Level = 250;
        userData.Exp = 28253490;
        userData.StaminaSingle = 999;
        userData.QuestSkipPoint = 300;

        this.ApiContext.PlayerDmodeInfos.Add(
            new DbPlayerDmodeInfo
            {
                ViewerId = newPlayer.ViewerId,
                Point1Quantity = 100_000_000,
                Point2Quantity = 100_000_000
            }
        );

        this.ApiContext.PlayerDmodeDungeons.Add(
            new DbPlayerDmodeDungeon { ViewerId = newPlayer.ViewerId }
        );

        this.ApiContext.PlayerDmodeExpeditions.Add(
            new DbPlayerDmodeExpedition { ViewerId = newPlayer.ViewerId }
        );

        await this.ApiContext.SaveChangesAsync();
        this.ApiContext.ChangeTracker.Clear();
    }

    private async Task SeedCache()
    {
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
