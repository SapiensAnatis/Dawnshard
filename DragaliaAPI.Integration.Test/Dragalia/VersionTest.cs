using DragaliaAPI.Models.Generated;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.VersionController"/>
/// </summary>
public class VersionTest : TestFixture
{
    public VersionTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Theory]
    [InlineData(1, "b1HyoeTFegeTexC0")]
    [InlineData(2, "y2XM6giU6zz56wCm")]
    public async Task GetResourceVersion_ReturnsCorrectResponse(
        int platform,
        string expectedVersion
    )
    {
        VersionGetResourceVersionData response = (
            await this.Client.PostMsgpack<VersionGetResourceVersionData>(
                "version/get_resource_version",
                new VersionGetResourceVersionRequest(platform, "whatever")
            )
        ).data;

        response.resource_version.Should().Be(expectedVersion);
    }
}
