using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DragaliaAPI.Photon.StateManager.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper;

    public CustomWebApplicationFactory()
    {
        this.testContainersHelper = new();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable(
            "RedisOptions__Hostname",
            this.testContainersHelper.RedisHost
        );
        Environment.SetEnvironmentVariable(
            "RedisOptions__Port",
            this.testContainersHelper.RedisPort.ToString()
        );
    }

    public Task InitializeAsync() => this.testContainersHelper.StartAsync();

    Task IAsyncLifetime.DisposeAsync() => this.testContainersHelper.StopAsync();

    protected override void Dispose(bool disposing)
    {
        Environment.SetEnvironmentVariable("RedisOptions__Hostname", null);
        Environment.SetEnvironmentVariable("RedisOptions__Port", null);
    }
}
