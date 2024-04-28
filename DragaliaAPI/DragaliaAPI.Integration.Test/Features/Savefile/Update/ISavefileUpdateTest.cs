using DragaliaAPI.Features.Savefile.Update;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Savefile.Update;

public class ISavefileUpdateTest : TestFixture
{
    private readonly IEnumerable<ISavefileUpdate> updates;

    public ISavefileUpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.updates = this.Services.GetServices<ISavefileUpdate>();
    }

    [Fact]
    public void ISavefileUpdate_HasExpectedCount()
    {
        // Update this test when adding a new update.
        this.updates.Should().HaveCount(20);
    }

    [Fact]
    public void ISavefileUpdate_NoDuplicateVersions()
    {
        this.updates.Should().OnlyHaveUniqueItems(x => x.SavefileVersion);
    }
}
