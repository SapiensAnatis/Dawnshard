using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace DragaliaAPI.Photon.StateManager.Test;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
            cfg.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["PhotonOptions:Token"] = "photontoken",
                    ["ConnectionStrings:Redis"] =
                        this.testContainersHelper.GetRedisConnectionString(),
                }
            )
        );

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
    }

    public async ValueTask InitializeAsync()
    {
        await this.testContainersHelper.StartAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await this.testContainersHelper.StopAsync();
    }
}
