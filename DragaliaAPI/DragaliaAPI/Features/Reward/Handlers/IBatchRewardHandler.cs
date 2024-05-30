using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Reward.Handlers;

public interface IBatchRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; }

    public Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(IDictionary<TKey, Entity> entity)
        where TKey : struct;
}
