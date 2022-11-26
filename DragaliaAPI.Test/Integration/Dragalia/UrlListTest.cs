using DragaliaAPI.Models.Generated;

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
    public async Task UrlList_ReturnsList()
    {
        WebviewVersionUrlListData response = (
            await client.PostMsgpack<WebviewVersionUrlListData>(
                "webview_version/url_list",
                new WebviewVersionUrlListRequest("region")
            )
        ).data;

        response.webview_url_list.Should().NotBeEmpty();
    }
}
