using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class DungeonRecordTest : TestFixture
{
    public DungeonRecordTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task Record_ReturnsExpectedResponse()
    {
        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                QuestData = MasterAsset.QuestData.Get(227100106)
            };

        string key;

        key = await this.Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        DungeonRecordRecordData response = (
            await this.Client.PostMsgpack<DungeonRecordRecordData>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    dungeon_key = key,
                    play_record = new PlayRecord
                    {
                        treasure_record = new List<AtgenTreasureRecord>(),
                        live_unit_no_list = new List<int>(),
                        damage_record = new List<AtgenDamageRecord>(),
                        dragon_damage_record = new List<AtgenDamageRecord>(),
                        battle_royal_record = new AtgenBattleRoyalRecord()
                    }
                }
            )
        ).data;

        // TODO: Add more asserts as we add logic into this endpoint
        response.ingame_result_data.dungeon_key.Should().Be(key);
        response.ingame_result_data.quest_id.Should().Be(227100106);

        response.update_data_list.user_data.Should().NotBeNull();
        response.update_data_list.quest_list.Should().NotBeNull();
    }
}
