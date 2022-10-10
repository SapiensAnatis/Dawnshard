using DragaliaAPI.Models.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");

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

            _connection.Open();
            services.AddDbContext<ApiContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            services.AddDistributedMemoryCache();

            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;

            var logger = scopedServices.GetRequiredService<
                ILogger<CustomWebApplicationFactory<TStartup>>
            >();
            var context = scopedServices.GetRequiredService<ApiContext>();
            var cache = scopedServices.GetRequiredService<IDistributedCache>();

            context.Database.EnsureCreated();

            context.DeviceAccounts.AddRange(TestUtils.GetDeviceAccountsSeed());
            context.SavefileUserData.AddRange(TestUtils.GetSavefilePlayerInfoSeed());
            context.SaveChanges();
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Seed the cache with a valid session, so that controllers can lookup database entries.
    /// </summary>
    public void SeedCache()
    {
        var cache = this.Services.GetRequiredService<IDistributedCache>();
        string sessionJson = """
                {
                    "SessionId": "session_id",
                    "DeviceAccountId": "logged_in_id"
                }
                """;
        cache.SetString(":session:session_id:session_id", sessionJson);
        cache.SetString(":session_id:device_account_id:logged_in_id", "session_id");
    }
}
