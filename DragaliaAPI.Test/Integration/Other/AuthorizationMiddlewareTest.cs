namespace DragaliaAPI.Test.Integration.Other;

public class AuthorizationMiddlewareTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public const string Endpoint = "/test";
    public const string AnonymousEndpoint = "/test/anonymous";

    public AuthorizationMiddlewareTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient();
    }

    [Fact]
    public async Task ValidSidHeader_ReturnsExpectedResponse()
    {
        client.DefaultRequestHeaders.Add("SID", "session_id");

        HttpResponseMessage response = await client.GetAsync(Endpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("OK");
    }

    [Fact]
    public async Task InvalidSidHeader_ReturnsSessionRefresh()
    {
        client.DefaultRequestHeaders.Add("SID", "invalid");

        HttpResponseMessage response = await client.GetAsync(Endpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Session-Expired");
        response.Headers.GetValues("Session-Expired").Should().ContainSingle().And.Contain("true");
    }

    [Fact]
    public async Task InvalidSidHeader_AllowAnonymous_ReturnsExpectedResponse()
    {
        client.DefaultRequestHeaders.Add("SID", "invalid");

        HttpResponseMessage response = await client.GetAsync(AnonymousEndpoint);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Be("OK");
    }
}
