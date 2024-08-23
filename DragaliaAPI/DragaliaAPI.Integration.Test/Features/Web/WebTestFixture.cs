namespace DragaliaAPI.Integration.Test.Features.Web;

public class WebTestFixture : TestFixture
{
    protected WebTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.MockBaasApi.Setup(x => x.GetUserId(It.IsAny<string>())).ReturnsAsync(DeviceAccountId);
    }
}
