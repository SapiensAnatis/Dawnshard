namespace DragaliaAPI.Features.Reward.Handlers;

public record struct GrantReturn(RewardGrantResult Result, Entity? ConvertedEntity = null);
