using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DragaliaAPI.Models;

namespace DragaliaAPI.Test.Integration.Other;

public class DragalipatchTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture fixture;
    private HttpClient client;

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

        DragalipatchConfig? config = await response.Content.ReadFromJsonAsync<DragalipatchConfig>();

        config.Should().NotBeNull();
        config
            .Should()
            .BeEquivalentTo(
                new DragalipatchConfig()
                {
                    Mode = "RAW",
                    CdnUrl = "https://github.com",
                    ConeshellKey = "password123",
                    UseUnifiedLogin = true,
                }
            );
    }
}
