namespace DragaliaAPI.Integration.Test.Features.Webview;

public class UrlListTest : TestFixture
{
    public UrlListTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task UrlList_ReturnsList()
    {
        WebviewVersionUrlListResponse response = (
            await this.Client.PostMsgpack<WebviewVersionUrlListResponse>(
                "webview_version/url_list",
                new WebviewVersionUrlListRequest("region"),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.WebviewUrlList.Should().NotBeEmpty();
    }
}
