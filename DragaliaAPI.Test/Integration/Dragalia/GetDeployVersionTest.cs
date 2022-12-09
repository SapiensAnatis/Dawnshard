using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Test.Integration.Dragalia;

[Collection("DragaliaIntegration")]
public class GetDeployVersionTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public GetDeployVersionTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task GetDeployVersion_ReturnsCorrectResponse()
    {
        DeployGetDeployVersionData response = (
            await client.PostMsgpack<DeployGetDeployVersionData>(
                "deploy/get_deploy_version",
                new DeployGetDeployVersionRequest()
            )
        ).data;

        response.deploy_hash.Should().Be("13bb2827ce9e6a66015ac2808112e3442740e862");
    }
}
