namespace DragaliaAPI.Features.Reward.V2.Handlers;

// ReSharper disable once TypeParameterCanBeVariant
public interface IRewardHandler<TReward>
    where TReward : struct, IReward
{
    public Task<GrantResult> GrantAsync(TReward reward);

    public Task<IDictionary<TKey, GrantResult>> GrantRangeAsync<TKey>(
        IDictionary<TKey, TReward> rewards
    )
        where TKey : struct;
}
