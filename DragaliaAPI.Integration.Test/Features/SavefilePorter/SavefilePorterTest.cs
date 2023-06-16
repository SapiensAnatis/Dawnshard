using DragaliaAPI.Features.SavefilePorter;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace DragaliaAPI.Integration.Test.Features.SavefilePorter;

public class SavefilePorterTest : TestFixture
{
    public SavefilePorterTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ISavefilePorter_Port_CanBeAppliedTwice()
    {
        IPlayerIdentityService identityService =
            this.Services.GetRequiredService<IPlayerIdentityService>();
        using IDisposable ctx = identityService.StartUserImpersonation(DeviceAccountId);

        // Test that each savefile porter is safe to apply over existing data.
        IEnumerable<ISavefilePorter> services = this.Services.GetServices<ISavefilePorter>();

        // Update this when adding a new porter
        services.Should().HaveCount(3);

        foreach (ISavefilePorter porter in services)
        {
            await porter.Port();
            await this.ApiContext.SaveChangesAsync();

            await porter.Invoking(x => x.Port()).Should().NotThrowAsync();
        }
    }
}
