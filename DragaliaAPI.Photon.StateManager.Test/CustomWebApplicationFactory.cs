using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IConnectionMultiplexer>();
            services.RemoveAll<IRedisConnectionProvider>();

            IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(
                $"{TestContainers.RedisHost}:{TestContainers.RedisPort}"
            );

            IRedisConnectionProvider provider = new RedisConnectionProvider(multiplexer);
            services.AddSingleton(provider);

            provider.Connection.CreateIndex(typeof(RedisGame));
        });
    }
}
