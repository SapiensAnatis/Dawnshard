using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
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
        Environment.SetEnvironmentVariable("RedisOptions__Hostname", TestContainers.RedisHost);
        Environment.SetEnvironmentVariable(
            "RedisOptions__Port",
            TestContainers.RedisPort.ToString()
        );
    }

    protected override void Dispose(bool disposing)
    {
        Environment.SetEnvironmentVariable("RedisOptions__Hostname", null);
        Environment.SetEnvironmentVariable("RedisOptions__Port", null);
    }
}
