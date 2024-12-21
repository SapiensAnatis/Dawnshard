using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Shared.Reward.Handlers;

public interface IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; }

    public Task<GrantReturn> Grant(Entity entity);
}
