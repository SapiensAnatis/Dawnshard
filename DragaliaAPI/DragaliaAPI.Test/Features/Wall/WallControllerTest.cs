using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Logging.Abstractions;

namespace DragaliaAPI.Test.Features.Wall;

public class WallControllerTest
{
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IClearPartyService> mockClearPartyService;
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IWallService> mockWallService;

    private readonly WallController wallController;

    public WallControllerTest()
    {
        mockUpdateDataService = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);
        mockClearPartyService = new(MockBehavior.Strict);
        mockDungeonService = new(MockBehavior.Strict);
        mockWallService = new(MockBehavior.Strict);

        wallController = new(
            mockUpdateDataService.Object,
            mockRewardService.Object,
            mockClearPartyService.Object,
            mockDungeonService.Object,
            mockWallService.Object,
            NullLogger<WallController>.Instance
        );
    }

    [Fact]
    public async Task GetMonthlyReward_ReturnsReward()
    {
        int questGroupId = 21601;
        int totalLevel = 2;
        DateTimeOffset lastRewardDate = DateTimeOffset.UtcNow;
        RewardStatus rewardStatus = RewardStatus.Received;

        AtgenUserWallRewardList rewardList =
            new()
            {
                QuestGroupId = questGroupId,
                SumWallLevel = totalLevel,
                LastRewardDate = lastRewardDate,
                RewardStatus = rewardStatus,
            };

        mockWallService.Setup(x => x.CheckWallInitialized()).ReturnsAsync(true);
        mockWallService.Setup(x => x.GetUserWallRewardList()).ReturnsAsync(rewardList);

        WallGetMonthlyRewardResponse data = (
            await wallController.GetMonthlyReward()
        ).GetData<WallGetMonthlyRewardResponse>()!;

        data.UserWallRewardList.Should().ContainSingle().Which.Should().BeEquivalentTo(rewardList);

        mockWallService.VerifyAll();
    }

    [Fact]
    public async Task ReceiveMonthlyReward_ReturnsRewards()
    {
        int questGroupId = 21601;
        int totalLevel = 2;
        DateTimeOffset lastRewardDate = DateTimeOffset.UtcNow;
        RewardStatus rewardStatus = RewardStatus.Received;

        AtgenUserWallRewardList rewardList =
            new()
            {
                QuestGroupId = questGroupId,
                SumWallLevel = totalLevel,
                LastRewardDate = lastRewardDate,
                RewardStatus = rewardStatus,
            };

        AtgenMonthlyWallReceiveList monthlyWallReceiveList =
            new()
            {
                QuestGroupId = WallService.WallQuestGroupId,
                IsReceiveReward = RewardStatus.Received,
            };
        IEnumerable<AtgenMonthlyWallReceiveList> monthlyWallReceiveListList = new[]
        {
            monthlyWallReceiveList,
        };

        List<AtgenBuildEventRewardEntityList> buildEventRewardEntityList =
            new()
            {
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Mana,
                    EntityId = 0,
                    EntityQuantity = 2500,
                },
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Rupies,
                    EntityId = 0,
                    EntityQuantity = 10000,
                },
            };

        DateTimeOffset lastClaimDate = DateTimeOffset.UtcNow.AddDays(-62);

        mockWallService.Setup(x => x.CheckWallInitialized()).ReturnsAsync(true);
        mockWallService
            .Setup(x => x.GetLastRewardDate())
            .ReturnsAsync(new DbWallRewardDate() { LastClaimDate = lastClaimDate });
        mockWallService.Setup(x => x.CheckCanClaimReward(lastClaimDate)).Returns(true);
        mockWallService.Setup(x => x.GetUserWallRewardList()).ReturnsAsync(rewardList);
        mockWallService.Setup(x => x.GetTotalWallLevel()).ReturnsAsync(totalLevel);
        mockWallService
            .Setup(x => x.GetMonthlyRewardEntityList(totalLevel))
            .Returns(buildEventRewardEntityList);
        mockWallService
            .Setup(x => x.GrantMonthlyRewardEntityList(buildEventRewardEntityList))
            .Returns(Task.CompletedTask);

        mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        mockUpdateDataService
            .Setup(x => x.SaveChangesAsync(TestContext.Current.CancellationToken))
            .ReturnsAsync(new UpdateDataList());

        WallReceiveMonthlyRewardResponse data = (
            await wallController.ReceiveMonthlyReward(TestContext.Current.CancellationToken)
        ).GetData<WallReceiveMonthlyRewardResponse>()!;

        data.UserWallRewardList.Should().ContainSingle().Which.Should().BeEquivalentTo(rewardList);
        data.MonthlyWallReceiveList.Should().BeEquivalentTo(monthlyWallReceiveListList);

        mockWallService.VerifyAll();
    }
}
