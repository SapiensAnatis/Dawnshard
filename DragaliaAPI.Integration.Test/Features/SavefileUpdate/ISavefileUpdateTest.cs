using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class ISavefileUpdateTest : TestFixture
{
    private readonly IEnumerable<ISavefileUpdate> updates;

    public ISavefileUpdateTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper)
    {
        updates = this.Services.GetServices<ISavefileUpdate>();
    }

    [Fact]
    public async Task ISavefileUpdate_Apply_CanBeAppliedTwice()
    {
        // Check that none of the savefile updates cause PK errors when writing over duplicate data.

        IPlayerIdentityService identityService =
            this.Services.GetRequiredService<IPlayerIdentityService>();
        using IDisposable ctx = identityService.StartUserImpersonation(DeviceAccountId);

        foreach (ISavefileUpdate update in updates)
        {
            await update.Apply();
            await this.ApiContext.SaveChangesAsync();

            await update.Invoking(x => x.Apply()).Should().NotThrowAsync();
        }
    }

    [Fact]
    public void ISavefileUpdate_HasExpectedCount()
    {
        // Update this test when adding a new update.
        this.updates.Should().HaveCount(2);
    }

    [Fact]
    public void ISavefileUpdate_NoDuplicateVersions()
    {
        this.updates.Should().OnlyHaveUniqueItems(x => x.SavefileVersion);
    }
}
