namespace DragaliaAPI.Test.Integration.Dragalia;

public class GetDeployVersionTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GetDeployVersionTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
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

        HttpResponseMessage response = await _client.PostAsync(
            "deploy/get_deploy_version",
            content
        );

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
