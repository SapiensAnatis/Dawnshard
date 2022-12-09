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
        DungeonSession mockSession =
            new()
            {
                DungeonId = 1,
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                AreaInfo = new List<DataQuestAreaInfo>()
                {
                    new("Main/01/MAIN_01_0104_01", "MAIN_01_0104_01")
                }
            };

        string key;

        using (IServiceScope scope = fixture.Services.CreateScope())
        {
            key = await this.fixture.Services
                .GetRequiredService<IDungeonService>()
                .StartDungeon(mockSession);
        }

        DungeonGetAreaOddsData response = (
            await this.client.PostMsgpack<DungeonGetAreaOddsData>(
                "/dungeon/get_area_odds",
                new DungeonGetAreaOddsRequest() { area_idx = 1, dungeon_key = key }
            )
        ).data;

        // there isn't too much to test here
        response.odds_info.area_index.Should().Be(1);
    }
}
