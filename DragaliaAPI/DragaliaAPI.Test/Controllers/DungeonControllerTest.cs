using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Test.Controllers;

public class DungeonControllerTest
{
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IOddsInfoService> mockOddsInfoService;
    private readonly Mock<IQuestService> mockQuestService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IMatchingService> mockMatchingService;
    private readonly Mock<IDungeonRecordHelperService> mockDungeonRecordHelperService;

    private readonly DungeonController dungeonController;

    public DungeonControllerTest()
    {
        this.mockDungeonService = new(MockBehavior.Strict);
        this.mockOddsInfoService = new(MockBehavior.Strict);
        this.mockQuestService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockMatchingService = new(MockBehavior.Strict);
        this.mockDungeonRecordHelperService = new(MockBehavior.Strict);

        this.dungeonController = new(
            this.mockDungeonService.Object,
            this.mockOddsInfoService.Object,
            this.mockQuestService.Object,
            this.mockUpdateDataService.Object,
            this.mockRewardService.Object,
            this.mockMatchingService.Object,
            this.mockDungeonRecordHelperService.Object
        );
    }

    [Fact]
    public async Task Fail_IsMultiFalse_ReturnsExpectedResponse()
    {
        int questId = 227060105;

        List<UserSupportList> userSupportList =
            new() { new() { support_chara = new() { chara_id = Charas.HalloweenLowen } } };

        List<AtgenHelperDetailList> supportDetailList =
            new()
            {
                new()
                {
                    is_friend = false,
                    viewer_id = 1,
                    get_mana_point = 50,
                }
            };

        this.mockDungeonService.Setup(x => x.FinishDungeon("my key"))
            .ReturnsAsync(
                new DungeonSession()
                {
                    QuestData = MasterAsset.QuestData.Get(questId),
                    Party = new List<PartySettingList>(),
                    IsMulti = false,
                    SupportViewerId = 4,
                }
            );

        this.mockDungeonRecordHelperService.Setup(x => x.ProcessHelperDataSolo(4))
            .ReturnsAsync((userSupportList, supportDetailList));

        DungeonFailData? response = (
            await this.dungeonController.Fail(new DungeonFailRequest() { dungeon_key = "my key" })
        ).GetData<DungeonFailData>();

        response.Should().NotBeNull();
        response!
            .Should()
            .BeEquivalentTo(
                new DungeonFailData()
                {
                    result = 1,
                    fail_helper_list = userSupportList,
                    fail_helper_detail_list = supportDetailList,
                    fail_quest_detail = new()
                    {
                        wall_id = 0,
                        wall_level = 0,
                        is_host = true,
                        quest_id = questId
                    }
                }
            );

        this.mockDungeonService.VerifyAll();
        this.mockDungeonRecordHelperService.VerifyAll();
    }

    [Fact]
    public async Task Fail_IsMultiTrue_RespondsExpectedResponse()
    {
        int questId = 227060105;

        List<UserSupportList> userSupportList =
            new() { new() { support_chara = new() { chara_id = Charas.HalloweenLowen } } };

        List<AtgenHelperDetailList> supportDetailList =
            new()
            {
                new()
                {
                    is_friend = false,
                    viewer_id = 1,
                    get_mana_point = 50,
                }
            };

        this.mockDungeonService.Setup(x => x.FinishDungeon("my key"))
            .ReturnsAsync(
                new DungeonSession()
                {
                    QuestData = MasterAsset.QuestData.Get(questId),
                    Party = new List<PartySettingList>(),
                    IsMulti = true,
                }
            );

        this.mockDungeonRecordHelperService.Setup(x => x.ProcessHelperDataMulti())
            .ReturnsAsync((userSupportList, supportDetailList));

        this.mockMatchingService.Setup(x => x.GetIsHost()).ReturnsAsync(false);

        DungeonFailData? response = (
            await this.dungeonController.Fail(new DungeonFailRequest() { dungeon_key = "my key" })
        ).GetData<DungeonFailData>();

        response.Should().NotBeNull();
        response!
            .Should()
            .BeEquivalentTo(
                new DungeonFailData()
                {
                    result = 1,
                    fail_helper_list = userSupportList,
                    fail_helper_detail_list = supportDetailList,
                    fail_quest_detail = new()
                    {
                        wall_id = 0,
                        wall_level = 0,
                        is_host = false,
                        quest_id = questId
                    }
                }
            );

        this.mockDungeonService.VerifyAll();
        this.mockMatchingService.VerifyAll();
        this.mockDungeonRecordHelperService.VerifyAll();
    }
}
