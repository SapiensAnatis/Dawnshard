using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.GetResourceVersionController"/>
/// </summary>
public class GetResourceVersionTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public GetResourceVersionTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task GetResourceVersion_ReturnsCorrectResponse()
    {
        VersionGetResourceVersionData response = (
            await client.PostMsgpack<VersionGetResourceVersionData>(
                "version/get_resource_version",
                new VersionGetResourceVersionRequest(0, "whatever")
            )
        ).data;

        response.resource_version.Should().Be("y2XM6giU6zz56wCm");
    }
}
