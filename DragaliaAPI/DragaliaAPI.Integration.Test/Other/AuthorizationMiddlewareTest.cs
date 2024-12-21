namespace DragaliaAPI.Integration.Test.Other;

public class AuthorizationMiddlewareTest : TestFixture
{
    private const string Endpoint = "emblem/get_list";

    public AuthorizationMiddlewareTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ValidSidHeader_ReturnsExpectedResponse()
    {
        this.Client.DefaultRequestHeaders.Clear();
        this.Client.DefaultRequestHeaders.Add("SID", this.SessionId);

        HttpResponseMessage response = await this.Client.PostAsync(
            Endpoint,
            null,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task InvalidSidHeader_ReturnsSessionRefresh()
    {
        this.Client.DefaultRequestHeaders.Clear();
        this.Client.DefaultRequestHeaders.Add("SID", "invalid");

        HttpResponseMessage response = await this.Client.PostAsync(
            Endpoint,
            null,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Session-Expired");
        response.Headers.GetValues("Session-Expired").Should().ContainSingle().And.Contain("true");
    }
}
