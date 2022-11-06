using DragaliaAPI.Models.Responses;

namespace DragaliaAPI.Test.Integration.Dragalia;

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
        GetResourceVersionResponse expectedResponse =
            new(new GetResourceVersionData(GetResourceVersionStatic.ResourceVersion));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync(
            "version/get_resource_version",
            content
        );

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
