using Castle.Components.DictionaryAdapter.Xml;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace DragaliaAPI.Integration.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly NpgsqlConnectionStringBuilder ConnectionStringBuilder =
        new()
        {
            Host = TestContainers.PostgresHost,
            Port = TestContainers.PostgresPort,
            Username = TestContainers.PostgresUser,
            Password = TestContainers.PostgresPassword,
            Database = TestContainers.PostgresDatabase,
            IncludeErrorDetail = true,
        };

    public Mock<IBaasApi> MockBaasApi { get; } = new();

    public Mock<IPhotonStateApi> MockPhotonStateApi { get; } = new();

    public Mock<IDateTimeProvider> MockDateTimeProvider { get; } = new();

    async Task IAsyncLifetime.InitializeAsync()
    {
        await this.SeedDatabase();
        await this.SeedCache();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(x => this.MockBaasApi.Object);
            services.AddScoped(x => this.MockPhotonStateApi.Object);
            services.AddScoped(x => this.MockDateTimeProvider.Object);
            services.Configure<LoginOptions>(x => x.UseBaasLogin = true);

            services.RemoveAll<DbContextOptions<ApiContext>>();
            services.RemoveAll<IDistributedCache>();

            services.AddDbContext<ApiContext>(
                opts => opts.UseNpgsql(ConnectionStringBuilder.ConnectionString)
            );
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = $"{TestContainers.RedisHost}:{TestContainers.RedisPort}";
                options.InstanceName = "RedisInstance";
            });
        });

        this.SetupMocks();
        builder.UseEnvironment("Testing");
    }

    private void SetupMocks()
    {
        this.MockBaasApi.Setup(x => x.GetKeys()).ReturnsAsync(TokenHelper.SecurityKeys);
    }

    /// <summary>
    /// Seed the cache with a valid session, so that controllers can lookup database entries.
    /// </summary>
    private async Task SeedCache()
    {
        IDistributedCache cache = this.Services.GetRequiredService<IDistributedCache>();

        Session session =
            new(
                TestFixture.SessionId,
                "id_token",
                TestFixture.DeviceAccountId,
                12,
                DateTimeOffset.MaxValue
            );
        await cache.SetStringAsync(
            ":session:session_id:session_id",
            JsonSerializer.Serialize(session)
        );
        await cache.SetStringAsync(
            ":session_id:device_account_id:logged_in_id",
            TestFixture.SessionId
        );
    }

    private async Task SeedDatabase()
    {
        ApiContext apiContext = this.Services.GetRequiredService<ApiContext>();
        await apiContext.Database.MigrateAsync();

        await using NpgsqlConnection connection = new(ConnectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        Respawner respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions()
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Table[] { new("__EFMigrationsHistory") },
            }
        );

        await respawner.ResetAsync(connection);

        ISavefileService savefileService = this.Services.GetRequiredService<ISavefileService>();
        IPlayerIdentityService playerIdentityService =
            this.Services.GetRequiredService<IPlayerIdentityService>();

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            TestFixture.DeviceAccountId
        );

        await savefileService.Create();

        apiContext.PlayerMaterials.AddRange(
            Enum.GetValues<Materials>()
                .Select(
                    x =>
                        new DbPlayerMaterial()
                        {
                            DeviceAccountId = TestFixture.DeviceAccountId,
                            MaterialId = x,
                            Quantity = 99999999
                        }
                )
        );

        apiContext.PlayerDragonGifts.AddRange(
            Enum.GetValues<DragonGifts>()
                .Select(
                    x =>
                        new DbPlayerDragonGift()
                        {
                            DeviceAccountId = TestFixture.DeviceAccountId,
                            DragonGiftId = x,
                            Quantity = x < DragonGifts.FourLeafClover ? 1 : 999
                        }
                )
        );

        IFortRepository fortRepository = this.Services.GetRequiredService<IFortRepository>();
        await fortRepository.InitializeFort();

        apiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                DeviceAccountId = TestFixture.DeviceAccountId,
                PlantId = FortPlants.Smithy,
                Level = 9
            }
        );

        DbPlayerUserData userData = (
            await apiContext.PlayerUserData.FindAsync(TestFixture.DeviceAccountId)
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
                DeviceAccountId = TestFixture.DeviceAccountId,
                Point1Quantity = 100_000_000,
                Point2Quantity = 100_000_000
            }
        );

        apiContext.PlayerDmodeDungeons.Add(
            new DbPlayerDmodeDungeon { DeviceAccountId = TestFixture.DeviceAccountId }
        );

        apiContext.PlayerDmodeExpeditions.Add(
            new DbPlayerDmodeExpedition { DeviceAccountId = TestFixture.DeviceAccountId }
        );

        await apiContext.SaveChangesAsync();
    }
}
