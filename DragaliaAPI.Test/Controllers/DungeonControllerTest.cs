using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Test.Controllers;

public class DungeonControllerTest
{
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IOddsInfoService> mockOddsInfoService;
    private readonly Mock<IQuestService> mockQuestService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IRewardService> mockRewardService;

    private readonly DungeonController dungeonController;

    public DungeonControllerTest()
    {
        this.mockDungeonService = new(MockBehavior.Strict);
        this.mockOddsInfoService = new(MockBehavior.Strict);
        this.mockQuestService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);

        this.dungeonController = new(
            this.mockDungeonService.Object,
            this.mockOddsInfoService.Object,
            this.mockQuestService.Object,
            this.mockUpdateDataService.Object,
            this.mockRewardService.Object
        );

        dungeonController.SetupMockContext();
    }

    [Fact]
    public async Task Fail_RespondsWithCorrectQuestId()
    {
        this.mockDungeonService
            .Setup(x => x.FinishDungeon("my key"))
            .ReturnsAsync(
                new DungeonSession()
                {
                    QuestData = MasterAsset.QuestData.Get(227060105),
                    Party = new List<PartySettingList>()
                }
            );

        DungeonFailData? response = (
            await this.dungeonController.Fail(new DungeonFailRequest() { dungeon_key = "my key" })
        ).GetData<DungeonFailData>();

        response.Should().NotBeNull();
        response!.fail_quest_detail.quest_id.Should().Be(227060105);

        this.mockDungeonService.VerifyAll();
    }
}
