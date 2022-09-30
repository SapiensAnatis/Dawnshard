using DragaliaAPI.Models.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor sqlDescriptor = services.Single(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApiContext>));

            ServiceDescriptor redisDescriptor = services.Single(
                d => d.ServiceType == typeof(IDistributedCache) && d.ImplementationType?.Name == "RedisCache");

            services.Remove(sqlDescriptor);
            services.Remove(redisDescriptor);
            

            services.AddDbContext<ApiContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            services.AddDistributedMemoryCache();

            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<ApiContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
            var cache = scopedServices.GetRequiredService<IDistributedCache>();

            try
            {
                TestUtils.InitializeDbForTests(db);
                // This doesn't appear to persist values into the tests, but works if used in the constructor of each test class.
                // TestUtils.InitializeCacheForTests(cache); 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the " +
                                    "database with test messages. Error: {Message}", ex.Message);
            }
        });
    }
}