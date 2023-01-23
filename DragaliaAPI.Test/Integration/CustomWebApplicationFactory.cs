using DragaliaAPI.Database;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DragaliaAPI.Test.Integration;

// TODO: create a test fixture which instead contains this and let the test fixture handle database updates etc
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public readonly Mock<IBaasRequestHelper> mockBaasRequestHelper = new();

    public readonly Mock<IOptionsMonitor<LoginOptions>> mockLoginOptions = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor sqlDescriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<ApiContext>)
            );

            ServiceDescriptor redisDescriptor = services.Single(
                d =>
                    d.ServiceType == typeof(IDistributedCache)
                    && (d.ImplementationType?.Name ?? "").Contains("Redis")
            );

            services.Remove(sqlDescriptor);
            services.Remove(redisDescriptor);

            string host = Environment.GetEnvironmentVariable("CI_PIPELINE") is not null
                ? "postgres-test"
                : "host.docker.internal";

            NpgsqlConnectionStringBuilder builder =
                new()
                {
                    Username = "test",
                    Password = "test",
                    Host = host,
                    Port = 9060,
                    IncludeErrorDetail = true,
                };

            services.AddDbContext<ApiContext>(
                options =>
                    options.UseNpgsql(builder.ConnectionString).EnableSensitiveDataLogging(true)
            );
            services.AddDistributedMemoryCache();

            services.AddScoped(x => mockBaasRequestHelper.Object);
            services.AddScoped(x => mockLoginOptions.Object);
        });

        builder.UseEnvironment("Testing");
    }
}
