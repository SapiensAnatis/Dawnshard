using DragaliaAPI.Database;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.MasterAsset;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Npgsql;
using Respawn;
using Respawn.Graph;
using StackExchange.Redis;

namespace DragaliaAPI.Integration.Test;

[UsedImplicitly]
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper;
    private HttpClient? client;
    private ConnectionMultiplexer? connectionMultiplexer;

    public CustomWebApplicationFactory()
    {
        this.testContainersHelper = new TestContainersHelper();
    }

    public string PostgresConnectionString => this.testContainersHelper.PostgresConnectionString;

    public Mock<IBaasApi> MockBaasApi { get; } = new();

    public Mock<IPhotonStateApi> MockPhotonStateApi { get; } = new();

    public Respawner? Respawner { get; private set; }
    
    public HttpClient Client => client ?? throw new InvalidOperationException("Client cannot be accessed before InitializeAsync.");
    
    public FakeTimeProvider MockTimeProvider { get; } = new();

    public async Task InitializeAsync()
    {
        await MasterAsset.LoadAsync(FeatureFlagUtils.AllEnabledFeatureManager);

        await this.testContainersHelper.StartAsync();

        using IServiceScope scope = this.Services.CreateScope();
        ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();
        await context.Database.MigrateAsync();

        await using NpgsqlConnection connection = new(this.PostgresConnectionString);
        await connection.OpenAsync();

        this.Respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions()
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = [new Table("__EFMigrationsHistory")],
            }
        );

        this.connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(
            new ConfigurationOptions()
            {
                EndPoints = new EndPointCollection
                {
                    { this.testContainersHelper.RedisHost, this.testContainersHelper.RedisPort },
                },
            }
        );
        
        this.client = this.CreateClient(new WebApplicationFactoryClientOptions()
        {
            BaseAddress = new Uri(
                "http://localhost/2.19.0_20220714193707/",
                UriKind.Absolute
            ),
        });
        
        client.DefaultRequestHeaders.Add(DragaliaHttpConstants.Headers.SessionId, "session_id");
        client.DefaultRequestHeaders.Add("Platform", "2");
        client.DefaultRequestHeaders.Add("Res-Ver", "y2XM6giU6zz56wCm");
    }

    public void ResetCache()
    {
        this.connectionMultiplexer?.GetDatabase().Execute("FLUSHALL");
    }

    async Task IAsyncLifetime.DisposeAsync() => await this.testContainersHelper.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(x => this.MockBaasApi.Object);
            services.AddScoped(x => this.MockPhotonStateApi.Object);

            services.RemoveAll<DbContextOptions<ApiContext>>();
            services.RemoveAll<IDistributedCache>();

            services.AddDbContext<ApiContext>(
                opts =>
                    opts.UseNpgsql(this.testContainersHelper.PostgresConnectionString)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging(),
                optionsLifetime: ServiceLifetime.Singleton
            );
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new()
                {
                    EndPoints = new()
                    {
                        {
                            this.testContainersHelper.RedisHost,
                            this.testContainersHelper.RedisPort
                        },
                    },
                };
                options.InstanceName = "RedisInstance";
            });

            services.PostConfigureAll<JwtBearerOptions>(opts =>
            {
                opts.Authority = null;
                opts.TokenValidationParameters = new()
                {
                    ValidIssuer = "LukeFZ",
                    ValidAudience = "baas-Id",
                    IssuerSigningKeys = TokenHelper.SecurityKeys,
                };
            });

            services.AddSingleton(this.MockTimeProvider);
        });

        builder.UseEnvironment("Testing");

        // Ensure we override any supplemental config
        builder.ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.Testing.json"));
        
    }
}
