namespace DragaliaAPI.Integration.Test.Dragalia;

public class DungeonTest : TestFixture
{
    public DungeonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetAreaOdds_ReturnsExpectedResponse()
    {
        string key = (
            await this.Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 2 },
                    quest_id = 100010306
                }
            )
        ).data.ingame_data.dungeon_key;

        DungeonGetAreaOddsData response = (
            await this.Client.PostMsgpack<DungeonGetAreaOddsData>(
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
            await this.Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 1 },
                    quest_id = 100010207
                }
            )
        ).data.ingame_data.dungeon_key;

        DungeonFailData response = (
            await this.Client.PostMsgpack<DungeonFailData>(
                "/dungeon/fail",
                new DungeonFailRequest() { dungeon_key = key }
            )
        ).data;

        response
            .fail_quest_detail.Should()
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
