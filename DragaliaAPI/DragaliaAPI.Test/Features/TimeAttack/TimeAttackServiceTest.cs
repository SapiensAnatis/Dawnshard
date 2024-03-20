using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DragaliaAPI.Test.Features.TimeAttack;

public class TimeAttackServiceTest
{
    private readonly ITimeAttackCacheService substituteCacheService;
    private readonly ITimeAttackRepository substituteRepository;
    private readonly IOptionsMonitor<TimeAttackOptions> substituteOptions;
    private readonly IQuestRepository substituteQuestRepository;
    private readonly IRewardService substituteRewardService;
    private readonly IPlayerIdentityService substituteIdentityService;
    private readonly ILogger<TimeAttackService> substituteLogger;

    private readonly TimeAttackService timeAttackService;

    public TimeAttackServiceTest()
    {
        this.substituteCacheService = Substitute.For<ITimeAttackCacheService>();
        this.substituteRepository = Substitute.For<ITimeAttackRepository>();
        this.substituteOptions = Substitute.For<IOptionsMonitor<TimeAttackOptions>>();
        this.substituteQuestRepository = Substitute.For<IQuestRepository>();
        this.substituteRewardService = Substitute.For<IRewardService>();
        this.substituteIdentityService = Substitute.For<IPlayerIdentityService>();
        this.substituteLogger = Substitute.For<ILogger<TimeAttackService>>();

        this.timeAttackService = new TimeAttackService(
            this.substituteCacheService,
            this.substituteRepository,
            this.substituteOptions,
            this.substituteQuestRepository,
            this.substituteRewardService,
            this.substituteIdentityService,
            this.substituteLogger
        );
    }

    [Fact]
    public void GetIsRankedQuest_RankedQuest_NotActive_ReturnsFalse()
    {
        this.substituteOptions.CurrentValue.Returns(new TimeAttackOptions() { GroupId = 2 });

        int questId = 210010103; // High Midgard Co-op (also used for TA)
        this.timeAttackService.GetIsRankedQuest(questId).Should().BeFalse();

        _ = this.substituteOptions.Received().CurrentValue;
    }

    [Fact]
    public void GetIsRankedQuest_RankedQuest_Active_ReturnsTrue()
    {
        this.substituteOptions.CurrentValue.Returns(new TimeAttackOptions() { GroupId = 2 });

        int questId = 227010104; // Volk's Wrath Ranked Solo
        this.timeAttackService.GetIsRankedQuest(questId).Should().BeTrue();

        _ = this.substituteOptions.Received().CurrentValue;
    }

    [Fact]
    public void GetIsRankedQuest_NonRankedQuest_ReturnsFalse()
    {
        this.substituteOptions.CurrentValue.Returns(new TimeAttackOptions() { GroupId = 2 });

        int questId = 100000101; // prologue, I think?
        this.timeAttackService.GetIsRankedQuest(questId).Should().BeFalse();

        _ = this.substituteOptions.DidNotReceive().CurrentValue;
    }
}
