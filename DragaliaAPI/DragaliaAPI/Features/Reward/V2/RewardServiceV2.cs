using DragaliaAPI.Features.Reward.V2.Handlers;

namespace DragaliaAPI.Features.Reward.V2;

internal class RewardServiceV2(DragonHandler dragonHandler)
{
    public Task<GrantResult> GrantAsync<TReward>(TReward reward)
        where TReward : IReward
    {
        if (typeof(TReward) == typeof(DragonReward))
            return dragonHandler.GrantAsync((DragonReward)(object)reward);

        throw new NotSupportedException($"Reward type {typeof(TReward).Name} not supported");
    }

    public Task<IDictionary<TKey, GrantResult>> GrantRangeAsync<TKey, TReward>(
        IDictionary<TKey, TReward> rewards
    )
        where TKey : struct
        where TReward : struct, IReward
    {
        if (typeof(TReward) == typeof(DragonReward))
            return dragonHandler.GrantRangeAsync((IDictionary<TKey, DragonReward>)rewards);

        throw new NotSupportedException($"Reward type {typeof(TReward).Name} not supported");
    }

    public async Task<IEnumerable<GrantResult>> GrantRangeAsync<TReward>(
        IEnumerable<TReward> rewards
    )
        where TReward : struct, IReward
    {
        IDictionary<int, TReward> inputDictionary = rewards
            .Select((x, index) => KeyValuePair.Create(index, x))
            .ToDictionary();

        IDictionary<int, GrantResult> outputDictionary = await GrantRangeAsync(inputDictionary);

        return outputDictionary.Values;
    }
}
