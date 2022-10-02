namespace DragaliaAPI.Test.Integration.Dragalia;

public class GetResourceVersionTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GetResourceVersionTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetResourceVersion_ReturnsCorrectResponse()
    {
        GetResourceVersionResponse expectedResponse = new(
            new GetResourceVersionData(GetResourceVersionStatic.ResourceVersion));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("version/get_resource_version", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}