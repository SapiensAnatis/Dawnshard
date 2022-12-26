using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.VersionController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class VersionTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public VersionTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Theory]
    [InlineData(1, "b1HyoeTFegeTexC0")]
    [InlineData(2, "y2XM6giU6zz56wCm")]
    public async Task GetResourceVersion_ReturnsCorrectResponse(
        int platform,
        string expectedVersion
    )
    {
        VersionGetResourceVersionData response = (
            await client.PostMsgpack<VersionGetResourceVersionData>(
                "version/get_resource_version",
                new VersionGetResourceVersionRequest(platform, "whatever")
            )
        ).data;

        response.resource_version.Should().Be(expectedVersion);
    }
}
