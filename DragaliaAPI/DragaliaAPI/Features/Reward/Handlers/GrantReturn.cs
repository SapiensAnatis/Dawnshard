namespace DragaliaAPI.Features.Reward.Handlers;

public readonly record struct GrantReturn(RewardGrantResult Result, Entity? ConvertedEntity = null)
{
    public static GrantReturn Added() => new(RewardGrantResult.Added);

    public static GrantReturn Converted(Entity convertedEntity) =>
        new(RewardGrantResult.Converted, convertedEntity);

    public static GrantReturn Limit() => new(RewardGrantResult.Limit);

    public static GrantReturn Discarded() => new(RewardGrantResult.Discarded);

    public static GrantReturn FailError() => new(RewardGrantResult.FailError);
}
