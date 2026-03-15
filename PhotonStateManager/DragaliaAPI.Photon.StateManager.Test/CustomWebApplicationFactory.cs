using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.XUnit3;

namespace DragaliaAPI.Photon.StateManager.Test;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper = new();

    public void SetTestOutputHelper(ITestOutputHelper testOutputHelper)
    {
        this.Services.GetRequiredService<XUnit3TestOutputSink>().TestOutputHelper =
            testOutputHelper;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(
                Options.Create(
                    new XUnit3TestOutputSinkOptions()
                    {
                        OutputTemplate =
                            "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}",
                    }
                )
            );
            services.AddSingleton<XUnit3TestOutputSink>();
        });
        builder.UseSerilog(
            (_, serviceProvider, config) =>
            {
                config.Enrich.FromLogContext();
                config.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

                config.WriteTo.XUnit3TestOutput(
                    serviceProvider.GetRequiredService<XUnit3TestOutputSink>()
                );
            }
        );

        return base.CreateHost(builder);
    }

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
