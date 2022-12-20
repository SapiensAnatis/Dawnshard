using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

[Collection("DragaliaIntegration")]
public class DungeonTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public DungeonTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task GetAreaOdds_ReturnsExpectedResponse()
    {
        string key = (
            await this.client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest() { party_no_list = new[] { 2 }, quest_id = 100010306 }
            )
        )
            .data
            .ingame_data
            .dungeon_key;

        DungeonGetAreaOddsData response = (
            await this.client.PostMsgpack<DungeonGetAreaOddsData>(
                "/dungeon/get_area_odds",
                new DungeonGetAreaOddsRequest() { area_idx = 1, dungeon_key = key }
            )
        ).data;

        // there isn't too much to test here
        response.odds_info.area_index.Should().Be(1);
    }

    [Fact]
    public async Task Fail_ReturnsCorrectResponse()
    {
        string key = (
            await this.client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest() { party_no_list = new[] { 1 }, quest_id = 100010207 }
            )
        )
            .data
            .ingame_data
            .dungeon_key;

        DungeonFailData response = (
            await this.client.PostMsgpack<DungeonFailData>(
                "/dungeon/fail",
                new DungeonFailRequest() { dungeon_key = key }
            )
        ).data;

        response.fail_quest_detail
            .Should()
            .BeEquivalentTo(
                new AtgenFailQuestDetail()
                {
                    is_host = true,
                    quest_id = 100010207,
                    wall_id = 0,
                    wall_level = 0
                }
            );
    }
}
