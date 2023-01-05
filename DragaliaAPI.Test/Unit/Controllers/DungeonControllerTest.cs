using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Test.Unit.Controllers;

public class DungeonControllerTest
{
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly DungeonController dungeonController;

    public DungeonControllerTest()
    {
        this.mockDungeonService = new(MockBehavior.Strict);

        this.dungeonController = new(this.mockDungeonService.Object);
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
