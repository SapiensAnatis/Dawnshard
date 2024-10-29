namespace DragaliaAPI.Integration.Test.Other;

public class AuthorizationMiddlewareTest : TestFixture
{
    // Avoid clearing the request headers of the shared client
    private readonly HttpClient httpClient;
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

        this.httpClient = this.CreateClient();
    }

    [Fact]
    public async Task ValidSidHeader_ReturnsExpectedResponse()
    {
        this.httpClient.DefaultRequestHeaders.Clear();
        this.httpClient.DefaultRequestHeaders.Add("SID", "session_id");

        HttpResponseMessage response = await this.httpClient.GetAsync(Endpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("OK");
    }

    [Fact]
    public async Task InvalidSidHeader_ReturnsSessionRefresh()
    {
        this.httpClient.DefaultRequestHeaders.Clear();
        this.httpClient.DefaultRequestHeaders.Add("SID", "invalid");

        HttpResponseMessage response = await this.httpClient.GetAsync(Endpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Session-Expired");
        response.Headers.GetValues("Session-Expired").Should().ContainSingle().And.Contain("true");
    }
}
