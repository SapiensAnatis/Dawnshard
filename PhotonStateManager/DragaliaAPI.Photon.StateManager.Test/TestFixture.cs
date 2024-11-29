using System.Net.Http.Headers;
using DragaliaAPI.Photon.StateManager.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Redis.OM.Contracts;


namespace DragaliaAPI.Photon.StateManager.Test;

[Collection(TestCollection.Name)]
public class TestFixture : IAsyncLifetime
{
    public TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        this.Client = factory
            .WithWebHostBuilder(builder =>
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
            )
            .CreateClient();

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            "photontoken"
        );

        this.RedisConnectionProvider =
            factory.Services.GetRequiredService<IRedisConnectionProvider>();
    }

    protected HttpClient Client { get; }

    protected IRedisConnectionProvider RedisConnectionProvider { get; }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

#pragma warning disable CA1816 // Call GC.SuppressFinalize correctly
    public async ValueTask DisposeAsync() =>
        await this
            .RedisConnectionProvider.RedisCollection<RedisGame>()
            .DeleteAsync(this.RedisConnectionProvider.RedisCollection<RedisGame>());
#pragma warning restore CA1816
}
