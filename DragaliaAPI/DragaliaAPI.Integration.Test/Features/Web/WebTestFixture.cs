namespace DragaliaAPI.Integration.Test.Features.Web;

public class WebTestFixture : TestFixture
{
    protected HttpClient HttpClient { get; private set; }

    protected WebTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.HttpClient = this.CreateClient();
    }

    protected void SetupMockBaas()
    {
        this.MockBaasApi.Setup(x => x.GetUserId(It.IsAny<string>())).ReturnsAsync(DeviceAccountId);
    }

    protected void AddTokenCookie()
    {
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );

        this.HttpClient.DefaultRequestHeaders.Add("Cookie", $"idToken={token}");
    }
}
