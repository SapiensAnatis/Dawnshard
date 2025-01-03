namespace DragaliaAPI.Features.Shared.Reward.Handlers;

public interface IBatchRewardHandler : IRewardHandler
{
    public Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(IDictionary<TKey, Entity> entities)
        where TKey : struct;

    async Task<GrantReturn> IRewardHandler.Grant(Entity entity)
    {
        Dictionary<int, Entity> dict = new() { [1] = entity };
        IDictionary<int, GrantReturn> result = await this.GrantRange(dict);

        return result.First().Value;
    }
}
