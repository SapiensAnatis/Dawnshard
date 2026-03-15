using DragaliaAPI.Photon.StateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using Redis.OM.Contracts;

namespace DragaliaAPI.Photon.StateManager.Test;

[Collection(TestCollection.Name)]
public class TestFixture : IAsyncLifetime
{
    public ITestOutputHelper TestOutputHelper { get; }

    public TestFixture(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        this.TestOutputHelper = testOutputHelper;
        factory.SetTestOutputHelper(this.TestOutputHelper);

        this.Client = factory.CreateClient();

        this.Client.DefaultRequestHeaders.Authorization = new("Bearer", "photontoken");

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
