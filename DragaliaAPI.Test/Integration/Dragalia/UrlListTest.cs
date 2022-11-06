using DragaliaAPI.Models.Responses;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class UrlListTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public UrlListTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task UrlList_ReturnsCorrectList()
    {
        WebviewUrlListResponse expectedResponse =
            new(new WebviewUrlListData(WebviewUrlListStatic.GetAllUrls("localhost")));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("webview_version/url_list", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
