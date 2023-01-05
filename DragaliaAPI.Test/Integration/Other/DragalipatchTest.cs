using System.Net.Http.Json;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Options;

namespace DragaliaAPI.Test.Integration.Other;

[Collection("DragaliaIntegration")]
public class DragalipatchTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public DragalipatchTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();
    }

    [Fact]
    public async Task Config_ReturnsExpectedJson()
    {
        HttpResponseMessage response = await this.client.GetAsync("/dragalipatch/config");

        response.IsSuccessStatusCode.Should().BeTrue();

        DragalipatchResponse? config =
            await response.Content.ReadFromJsonAsync<DragalipatchResponse>();

        config.Should().NotBeNull();
        config
            .Should()
            .BeEquivalentTo(
                new DragalipatchResponse()
                {
                    Mode = "RAW",
                    CdnUrl = "https://github.com",
                    ConeshellKey = "password123",
                    UseUnifiedLogin = true,
                }
            );
    }
}
