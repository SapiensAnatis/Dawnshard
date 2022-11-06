using DragaliaAPI.Models.Responses;

namespace DragaliaAPI.Test.Integration.Dragalia;

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
        GetDeployVersionResponse expectedResponse =
            new(new GetDeployVersionData(GetDeployVersionStatic.DeployHash));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("deploy/get_deploy_version", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
