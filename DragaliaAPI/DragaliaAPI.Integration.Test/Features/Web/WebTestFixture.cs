namespace DragaliaAPI.Integration.Test.Features.Web;

public class WebTestFixture : TestFixture
{
    protected WebTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper) { }

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

        this.Client.DefaultRequestHeaders.Add("Cookie", $"idToken={token}");
    }
}
