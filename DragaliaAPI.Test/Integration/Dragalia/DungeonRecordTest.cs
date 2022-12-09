using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class DungeonRecordTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public DungeonRecordTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task Record_ReturnsExpectedResponse()
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

        DungeonRecordRecordData response = (
            await this.client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest() { dungeon_key = key }
            )
        ).data;

        // TODO: Add more asserts as we add logic into this endpoint
        response.ingame_result_data.dungeon_key.Should().Be(key);
        response.ingame_result_data.quest_id.Should().Be(1);

        response.update_data_list.user_data.Should().NotBeNull();
        response.update_data_list.quest_list.Should().NotBeNull();
    }
}
