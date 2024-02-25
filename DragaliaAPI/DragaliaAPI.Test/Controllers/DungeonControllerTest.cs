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
            new() { new() { SupportChara = new() { CharaId = Charas.HalloweenLowen } } };

        List<AtgenHelperDetailList> supportDetailList =
            new()
            {
                new()
                {
                    IsFriend = false,
                    ViewerId = 1,
                    GetManaPoint = 50,
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

        DungeonFailResponse? response = (
            await this.dungeonController.Fail(new DungeonFailRequest() { DungeonKey = "my key" })
        ).GetData<DungeonFailResponse>();

        response.Should().NotBeNull();
        response!
            .Should()
            .BeEquivalentTo(
                new DungeonFailResponse()
                {
                    Result = 1,
                    FailHelperList = userSupportList,
                    FailHelperDetailList = supportDetailList,
                    FailQuestDetail = new()
                    {
                        WallId = 0,
                        WallLevel = 0,
                        IsHost = true,
                        QuestId = questId
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
            new() { new() { SupportChara = new() { CharaId = Charas.HalloweenLowen } } };

        List<AtgenHelperDetailList> supportDetailList =
            new()
            {
                new()
                {
                    IsFriend = false,
                    ViewerId = 1,
                    GetManaPoint = 50,
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

        DungeonFailResponse? response = (
            await this.dungeonController.Fail(new DungeonFailRequest() { DungeonKey = "my key" })
        ).GetData<DungeonFailResponse>();

        response.Should().NotBeNull();
        response!
            .Should()
            .BeEquivalentTo(
                new DungeonFailResponse()
                {
                    Result = 1,
                    FailHelperList = userSupportList,
                    FailHelperDetailList = supportDetailList,
                    FailQuestDetail = new()
                    {
                        WallId = 0,
                        WallLevel = 0,
                        IsHost = false,
                        QuestId = questId
                    }
                }
            );

        this.mockDungeonService.VerifyAll();
        this.mockMatchingService.VerifyAll();
        this.mockDungeonRecordHelperService.VerifyAll();
    }
}
