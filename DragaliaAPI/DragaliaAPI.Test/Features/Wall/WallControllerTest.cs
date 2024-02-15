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
                quest_group_id = questGroupId,
                sum_wall_level = totalLevel,
                last_reward_date = lastRewardDate,
                reward_status = rewardStatus
            };
        IEnumerable<AtgenUserWallRewardList> userRewardList = new[] { rewardList };

        mockWallService
            .Setup(x => x.GetUserWallRewardList(totalLevel, rewardStatus))
            .Returns(userRewardList);

        mockWallService.Setup(x => x.GetTotalWallLevel()).ReturnsAsync(totalLevel);

        WallGetMonthlyRewardData data = (
            await wallController.GetMonthlyReward()
        ).GetData<WallGetMonthlyRewardData>()!;

        data.user_wall_reward_list.Should().BeEquivalentTo(userRewardList);

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
                quest_group_id = questGroupId,
                sum_wall_level = totalLevel,
                last_reward_date = lastRewardDate,
                reward_status = rewardStatus
            };
        IEnumerable<AtgenUserWallRewardList> userRewardList = new[] { rewardList };

        AtgenMonthlyWallReceiveList monthlyWallReceiveList =
            new()
            {
                quest_group_id = WallService.WallQuestGroupId,
                is_receive_reward = RewardStatus.Received
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
                    entity_type = EntityTypes.Mana,
                    entity_id = 0,
                    entity_quantity = 2500
                },
                new AtgenBuildEventRewardEntityList()
                {
                    entity_type = EntityTypes.Rupies,
                    entity_id = 0,
                    entity_quantity = 10000
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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(new UpdateDataList());

        WallReceiveMonthlyRewardData data = (
            await wallController.ReceiveMonthlyReward(new WallReceiveMonthlyRewardRequest())
        ).GetData<WallReceiveMonthlyRewardData>()!;

        data.user_wall_reward_list.Should().BeEquivalentTo(userRewardList);
        data.monthly_wall_receive_list.Should().BeEquivalentTo(monthlyWallReceiveListList);

        mockWallService.VerifyAll();
    }
}
