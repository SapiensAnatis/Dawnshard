using System.Net.Http.Json;
using DragaliaAPI.Models;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Other;

public class DragalipatchTest : TestFixture
{
    public DragalipatchTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task Config_ReturnsExpectedJson()
    {
        HttpResponseMessage response = await this.Client.GetAsync("/dragalipatch/config");

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
