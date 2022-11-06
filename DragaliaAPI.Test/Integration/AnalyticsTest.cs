namespace DragaliaAPI.Test.Integration;

public class AnalyticsTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AnalyticsTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task AnalyticsConfigController_Get_ReturnsOK()
    {
        HttpResponseMessage response = await _client.GetAsync("bigdata/v1/analytics/events/config");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task AnalyticsController_Post_ReturnsOK()
    {
        HttpResponseMessage response = await _client.PostAsync("bigdata/v1/analytics", null);

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
