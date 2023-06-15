using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
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

namespace DragaliaAPI.Integration.Test;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public CustomWebApplicationFactory()
    {
        this.SeedDatabase(this.Services);
        this.SeedCache(this.Services);
    }

    public Mock<IBaasApi> MockBaasApi { get; } = new();

    public Mock<IPhotonStateApi> MockPhotonStateApi { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(x => this.MockBaasApi.Object);
            services.AddScoped(x => this.MockPhotonStateApi.Object);
            services.Configure<LoginOptions>(x => x.UseBaasLogin = true);

            NpgsqlConnectionStringBuilder connectionStringBuilder =
                new()
                {
                    Host = TestContainers.PostgresHost,
                    Port = TestContainers.PostgresPort,
                    Username = TestContainers.PostgresUser,
                    Password = TestContainers.PostgresPassword
                };

            services.RemoveAll<DbContextOptions<ApiContext>>();
            services.RemoveAll<IDistributedCache>();

            services.AddDbContext<ApiContext>(
                opts => opts.UseNpgsql(connectionStringBuilder.ConnectionString)
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
    private void SeedCache(IServiceProvider provider)
    {
        IDistributedCache cache = this.Services.GetRequiredService<IDistributedCache>();

        object session = new { TestFixture.SessionId, TestFixture.DeviceAccountId };
        cache.SetString(":session:session_id:session_id", JsonSerializer.Serialize(session));
        cache.SetString(":session_id:device_account_id:logged_in_id", TestFixture.SessionId);
    }

    private void SeedDatabase(IServiceProvider provider)
    {
        ISavefileService savefileService = provider.GetRequiredService<ISavefileService>();
        IPlayerIdentityService playerIdentityService =
            provider.GetRequiredService<IPlayerIdentityService>();
        ApiContext apiContext = provider.GetRequiredService<ApiContext>();

        apiContext.Database.EnsureDeleted();
        apiContext.Database.EnsureCreated();

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(
            TestFixture.DeviceAccountId
        );

        savefileService.CreateBase().Wait();

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

        IFortRepository fortRepository = provider.GetRequiredService<IFortRepository>();
        fortRepository.InitializeFort().Wait();

        apiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                DeviceAccountId = TestFixture.DeviceAccountId,
                PlantId = FortPlants.Smithy,
                Level = 9
            }
        );

        apiContext.PlayerUserData.Find(TestFixture.DeviceAccountId)!.Coin = 100_000_000;
        apiContext.PlayerUserData.Find(TestFixture.DeviceAccountId)!.DewPoint = 100_000_000;
        apiContext.SaveChanges();
    }
}
