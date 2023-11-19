using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Features.Wall;

public class WallRecordControllerTest
{
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IWallRepository> mockWallRepository;
    private readonly Mock<IWallService> mockWallService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IPresentService> mockPresentService;
    private readonly Mock<IDungeonRecordHelperService> mockDungeonRecordHelperService;
    private readonly Mock<ILogger<WallRecordController>> mockLogger;

    private readonly WallRecordController wallRecordController;

    public WallRecordControllerTest()
    {
        mockUpdateDataService = new(MockBehavior.Strict);
        mockWallRepository = new(MockBehavior.Strict);
        mockWallService = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);
        mockDungeonService = new(MockBehavior.Strict);
        mockPresentService = new(MockBehavior.Strict);
        mockDungeonRecordHelperService = new(MockBehavior.Strict);
        mockLogger = new(MockBehavior.Loose);

        wallRecordController = new(
            mockUpdateDataService.Object,
            mockWallRepository.Object,
            mockWallService.Object,
            mockRewardService.Object,
            mockDungeonService.Object,
            mockPresentService.Object,
            mockDungeonRecordHelperService.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task Record_ReturnsData()
    {
        string dungeonKey = "key";
        List<PartySettingList> party = new();
        int wallId = WallService.FlameWallId;
        int wallLevel = 1;
        ulong supportViewerId = 1000;
        List<UserSupportList> helperList = new();
        List<AtgenHelperDetailList> helperDetailList = new();

        DungeonSession session =
            new()
            {
                QuestData = MasterAsset.QuestData[0],
                Party = party,
                WallId = wallId,
                WallLevel = wallLevel + 1, // Client passes (db wall level + 1)
                SupportViewerId = supportViewerId
            };

        DbPlayerQuestWall playerQuestWall =
            new()
            {
                ViewerId = 1,
                IsStartNextLevel = true,
                WallId = wallId,
                WallLevel = wallLevel
            };

        mockDungeonService.Setup(x => x.FinishDungeon(dungeonKey)).ReturnsAsync(session);

        mockWallRepository.Setup(x => x.GetQuestWall(wallId)).ReturnsAsync(playerQuestWall);

        mockWallService.Setup(x => x.LevelupQuestWall(wallId)).Returns(Task.CompletedTask);

        mockDungeonRecordHelperService
            .Setup(x => x.ProcessHelperDataSolo(supportViewerId))
            .ReturnsAsync((helperList, helperDetailList));

        mockRewardService
            .Setup(x => x.GrantReward(WallRecordController.GoldCrystals))
            .ReturnsAsync(RewardGrantResult.Added);

        mockRewardService
            .Setup(x => x.GrantReward(WallRecordController.Rupies))
            .ReturnsAsync(RewardGrantResult.Added);

        mockRewardService
            .Setup(x => x.GrantReward(WallRecordController.Mana))
            .ReturnsAsync(RewardGrantResult.Added);

        mockPresentService.Setup(x => x.AddPresent(WallRecordController.WyrmitesPresent));

        mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(new UpdateDataList());

        WallRecordRecordData data = (
            await wallRecordController.Record(new WallRecordRecordRequest(wallId, dungeonKey))
        ).GetData<WallRecordRecordData>()!;

        AtgenPlayWallDetail dataPlayWallDetail =
            new()
            {
                wall_id = wallId,
                after_wall_level = wallLevel + 1,
                before_wall_level = wallLevel
            };

        AtgenWallDropReward dataWallDropReward =
            new()
            {
                reward_entity_list = new[]
                {
                    WallRecordController.GoldCrystals.ToBuildEventRewardEntityList()
                },
                take_coin = WallRecordController.Rupies.Quantity,
                take_mana = WallRecordController.Mana.Quantity
            };

        IEnumerable<AtgenBuildEventRewardEntityList> dataWallClearRewardList = new[]
        {
            WallRecordController.Wyrmites.ToBuildEventRewardEntityList()
        };

        data.play_wall_detail.Should().BeEquivalentTo(dataPlayWallDetail);
        data.wall_clear_reward_list.Should().BeEquivalentTo(dataWallClearRewardList);
        data.wall_drop_reward.Should().BeEquivalentTo(dataWallDropReward);
    }
}
