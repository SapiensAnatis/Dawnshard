using DragaliaAPI.Database;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Integration;

// TODO: create a test fixture which instead contains this and let the test fixture handle database updates etc
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly SqliteConnection connection = new("Filename=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor sqlDescriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<ApiContext>)
            );

            ServiceDescriptor redisDescriptor = services.Single(
                d =>
                    d.ServiceType == typeof(IDistributedCache)
                    && d.ImplementationType?.Name == "RedisCache"
            );

            services.Remove(sqlDescriptor);
            services.Remove(redisDescriptor);

            connection.Open();
            services.AddDbContext<ApiContext>(options => options.UseSqlite(connection));

            services.AddDistributedMemoryCache();

            ApiContext context = services
                .BuildServiceProvider()
                .CreateScope()
                .ServiceProvider.GetRequiredService<ApiContext>();

            context.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}
