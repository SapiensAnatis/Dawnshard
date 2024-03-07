namespace DragaliaAPI.Integration.Test.Other;

public class AuthorizationMiddlewareTest : TestFixture
{
    private const string Endpoint = "test";

    public AuthorizationMiddlewareTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper)
    {
#if !DEBUG && !TEST
        throw new InvalidOperationException(
            "These tests must be run in a debug build as they use a conditionally compiled controller"
        );
#endif
    }

    [Fact]
    public async Task ValidSidHeader_ReturnsExpectedResponse()
    {
        this.Client.DefaultRequestHeaders.Clear();
        this.Client.DefaultRequestHeaders.Add("SID", "session_id");

        HttpResponseMessage response = await this.Client.GetAsync(Endpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("OK");
    }

    [Fact]
    public async Task InvalidSidHeader_ReturnsSessionRefresh()
    {
        this.Client.DefaultRequestHeaders.Clear();
        this.Client.DefaultRequestHeaders.Add("SID", "invalid");

        HttpResponseMessage response = await this.Client.GetAsync(Endpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Session-Expired");
        response.Headers.GetValues("Session-Expired").Should().ContainSingle().And.Contain("true");
    }
}
