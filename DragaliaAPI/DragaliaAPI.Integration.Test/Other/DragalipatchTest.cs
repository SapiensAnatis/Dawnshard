using System.Net.Http.Json;
using DragaliaAPI.Models;

namespace DragaliaAPI.Integration.Test.Other;

public class DragalipatchTest : TestFixture
{
    public DragalipatchTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        // Don't use /api prefix
        this.Client.BaseAddress = new Uri("http://localhost/");
    }

    [Fact]
    public async Task Config_ReturnsExpectedJson()
    {
        HttpResponseMessage response = await this.Client.GetAsync("dragalipatch/config");

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
