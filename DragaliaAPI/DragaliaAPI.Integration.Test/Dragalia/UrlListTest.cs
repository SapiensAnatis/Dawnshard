namespace DragaliaAPI.Integration.Test.Dragalia;

public class UrlListTest : TestFixture
{
    public UrlListTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task UrlList_ReturnsList()
    {
        WebviewVersionUrlListData response = (
            await this.Client.PostMsgpack<WebviewVersionUrlListData>(
                "webview_version/url_list",
                new WebviewVersionUrlListRequest("region")
            )
        ).data;

        response.webview_url_list.Should().NotBeEmpty();
    }
}
