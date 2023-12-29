using DragaliaAPI.Database;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DragaliaAPI.Integration.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly TestContainersHelper testContainersHelper;

    public CustomWebApplicationFactory(TestContainersHelper testContainersHelper)
    {
        this.testContainersHelper = testContainersHelper;

        this.MockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(() => DateTimeOffset.UtcNow);
    }

    public string PostgresConnectionString => this.testContainersHelper.PostgresConnectionString;

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

    protected Mock<IBaasApi> MockBaasApi { get; } = new();

    protected Mock<IPhotonStateApi> MockPhotonStateApi { get; } = new();

    protected Mock<IDateTimeProvider> MockDateTimeProvider { get; } = new();
}
