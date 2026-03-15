using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace DragaliaAPI.Integration.Test;

/// <summary>
/// Fixture classes for tests that need to mock the system time.
/// <remarks>
/// Creates a separate server with a FakeTimeProvider injected, so that these tests don't interfere with other tests
/// that rely on the system time being the actual real-world time.
/// </remarks>
/// </summary>
public class FakeTimeProviderTestFixture : TestFixture
{
    protected FakeTimeProvider FakeTimeProvider { get; } = new(DateTimeOffset.UtcNow);

    protected FakeTimeProviderTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        // This calls WithWebHostBuilder internally which spins off a new server
        this.Client = this.CreateClient(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<TimeProvider>(this.FakeTimeProvider);
            });
        });
    }
}
