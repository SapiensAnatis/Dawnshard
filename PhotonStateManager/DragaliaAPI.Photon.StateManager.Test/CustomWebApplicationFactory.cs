﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Photon.StateManager.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper;

    public CustomWebApplicationFactory()
    {
        this.testContainersHelper = new();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
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

    public Task InitializeAsync() => this.testContainersHelper.StartAsync();

    Task IAsyncLifetime.DisposeAsync() => this.testContainersHelper.StopAsync();
}
