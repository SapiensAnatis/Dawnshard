using DragaliaAPI.Database;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;

namespace DragaliaAPI.Integration.Test;

[UsedImplicitly]
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper;

    public CustomWebApplicationFactory(TestContainersHelper testContainersHelper)
    {
        this.testContainersHelper = testContainersHelper;
    }

    public string PostgresConnectionString => this.testContainersHelper.PostgresConnectionString;

    public Mock<IBaasApi> MockBaasApi { get; } = new();

    public Mock<IPhotonStateApi> MockPhotonStateApi { get; } = new();

    public Mock<IDateTimeProvider> MockDateTimeProvider { get; } = new();

    public Respawner? Respawner { get; private set; }

    public async Task InitializeAsync()
    {
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
                TablesToIgnore = [new("__EFMigrationsHistory")],
            }
        );
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;

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
                opts =>
                    opts.UseNpgsql(this.testContainersHelper.PostgresConnectionString)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging(),
                optionsLifetime: ServiceLifetime.Singleton
            );
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    $"{this.testContainersHelper.RedisHost}:{this.testContainersHelper.RedisPort}";
                options.InstanceName = "RedisInstance";
            });
        });

        builder.UseEnvironment("Testing");
    }
}
