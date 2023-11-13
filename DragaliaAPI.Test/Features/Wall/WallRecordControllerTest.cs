using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.CodeAnalysis.CSharp;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Features.Present;
using Castle.Core.Logging;
using DragaliaAPI.Features.Dungeon.Record;
using Microsoft.Extensions.Logging;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Test.Features.Wall;

public class WallRecordControllerTest
{
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IWallRepository> mockWallRepository;
    private readonly Mock<IWallService> mockWallService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IPresentService> mockPresentService;
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
        mockLogger = new(MockBehavior.Loose);

        wallRecordController = new(
            mockUpdateDataService.Object,
            mockWallRepository.Object,
            mockWallService.Object,
            mockRewardService.Object,
            mockDungeonService.Object,
            mockPresentService.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task Record_ReturnsData()
    {
        string dungeonKey = "key";
        List<PartySettingList> party = new();
        int wallId = WallService.FlameWallId;

        DungeonSession session = new() { QuestData = MasterAsset.QuestData[0], Party = party, };

        DbPlayerQuestWall playerQuestWall =
            new()
            {
                DeviceAccountId = "cool account",
                IsStartNextLevel = true,
                WallId = wallId,
                WallLevel = 1
            };

        mockDungeonService.Setup(x => x.FinishDungeon(dungeonKey)).ReturnsAsync(session);

        mockWallRepository.Setup(x => x.GetQuestWall(wallId)).ReturnsAsync(playerQuestWall);

        mockWallService.Setup(x => x.LevelupQuestWall(wallId)).Returns(Task.CompletedTask);

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
                after_wall_level = 2,
                before_wall_level = 1
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
