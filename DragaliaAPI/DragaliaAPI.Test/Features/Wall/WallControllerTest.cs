using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;

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
            mockWallService.Object
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
                RewardStatus = rewardStatus
            };
        IEnumerable<AtgenUserWallRewardList> userRewardList = new[] { rewardList };

        mockWallService
            .Setup(x => x.GetUserWallRewardList(totalLevel, rewardStatus))
            .Returns(userRewardList);

        mockWallService.Setup(x => x.GetTotalWallLevel()).ReturnsAsync(totalLevel);

        WallGetMonthlyRewardResponse data = (
            await wallController.GetMonthlyReward()
        ).GetData<WallGetMonthlyRewardResponse>()!;

        data.UserWallRewardList.Should().BeEquivalentTo(userRewardList);

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
                RewardStatus = rewardStatus
            };
        IEnumerable<AtgenUserWallRewardList> userRewardList = new[] { rewardList };

        AtgenMonthlyWallReceiveList monthlyWallReceiveList =
            new()
            {
                QuestGroupId = WallService.WallQuestGroupId,
                IsReceiveReward = RewardStatus.Received
            };
        IEnumerable<AtgenMonthlyWallReceiveList> monthlyWallReceiveListList = new[]
        {
            monthlyWallReceiveList
        };

        List<AtgenBuildEventRewardEntityList> buildEventRewardEntityList =
            new()
            {
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Mana,
                    EntityId = 0,
                    EntityQuantity = 2500
                },
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Rupies,
                    EntityId = 0,
                    EntityQuantity = 10000
                }
            };

        mockWallService
            .Setup(x => x.GetUserWallRewardList(totalLevel, rewardStatus))
            .Returns(userRewardList);

        mockWallService.Setup(x => x.GetTotalWallLevel()).ReturnsAsync(totalLevel);

        mockWallService
            .Setup(x => x.GetMonthlyRewardEntityList(totalLevel))
            .Returns(buildEventRewardEntityList);

        mockWallService
            .Setup(x => x.GrantMonthlyRewardEntityList(buildEventRewardEntityList))
            .Returns(Task.CompletedTask);

        mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        mockUpdateDataService
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(new UpdateDataList());

        WallReceiveMonthlyRewardResponse data = (
            await wallController.ReceiveMonthlyReward(
                new WallReceiveMonthlyRewardRequest(),
                default
            )
        ).GetData<WallReceiveMonthlyRewardResponse>()!;

        data.UserWallRewardList.Should().BeEquivalentTo(userRewardList);
        data.MonthlyWallReceiveList.Should().BeEquivalentTo(monthlyWallReceiveListList);

        mockWallService.VerifyAll();
    }
}
