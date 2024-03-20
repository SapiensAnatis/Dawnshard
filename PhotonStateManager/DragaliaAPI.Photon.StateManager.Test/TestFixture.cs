using System.Net.Http.Headers;
using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Redis.OM.Contracts;
using Xunit.Abstractions;

namespace DragaliaAPI.Photon.StateManager.Test;

[Collection(TestCollection.Name)]
public class TestFixture : IAsyncLifetime
{
    private const string PhotonToken = "photontoken";

    public TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        this.Client = factory
            .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddXUnit(outputHelper);
                    })
            )
            .CreateClient();

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            PhotonToken
        );

        Environment.SetEnvironmentVariable("PHOTON_TOKEN", PhotonToken);

        this.RedisConnectionProvider =
            factory.Services.GetRequiredService<IRedisConnectionProvider>();
    }

    protected HttpClient Client { get; }

    protected IRedisConnectionProvider RedisConnectionProvider { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() =>
        await this
            .RedisConnectionProvider.RedisCollection<RedisGame>()
            .DeleteAsync(this.RedisConnectionProvider.RedisCollection<RedisGame>());
}
