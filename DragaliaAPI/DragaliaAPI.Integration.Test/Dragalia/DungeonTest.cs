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
                    PartyNoList = new List<int>() { 2 },
                    QuestId = 100010306
                }
            )
        ).data.IngameData.DungeonKey;

        DungeonGetAreaOddsData response = (
            await this.Client.PostMsgpack<DungeonGetAreaOddsData>(
                "/dungeon/get_area_odds",
                new DungeonGetAreaOddsRequest() { AreaIdx = 1, DungeonKey = key }
            )
        ).data;

        // there isn't too much to test here
        response.OddsInfo.AreaIndex.Should().Be(1);
    }

    [Fact]
    public async Task Fail_ReturnsCorrectResponse()
    {
        string key = (
            await this.Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 1 },
                    QuestId = 100010207
                }
            )
        ).data.IngameData.DungeonKey;

        DungeonFailData response = (
            await this.Client.PostMsgpack<DungeonFailData>(
                "/dungeon/fail",
                new DungeonFailRequest() { DungeonKey = key }
            )
        ).data;

        response
            .FailQuestDetail.Should()
            .BeEquivalentTo(
                new AtgenFailQuestDetail()
                {
                    IsHost = true,
                    QuestId = 100010207,
                    WallId = 0,
                    WallLevel = 0
                }
            );
    }
}
