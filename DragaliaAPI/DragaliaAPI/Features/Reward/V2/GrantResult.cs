namespace DragaliaAPI.Features.Reward.V2;

public readonly record struct GrantResult(
    RewardGrantResult Result,
    IReward? ConvertedReward = null
);
