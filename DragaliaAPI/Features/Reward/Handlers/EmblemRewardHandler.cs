using System.Collections.Immutable;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Reward.Handlers;

public class EmblemRewardHandler(IEmblemRepository repository) : IRewardHandler
{
    public ImmutableArray<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.Title);

    public async Task<GrantReturn> Grant(Entity entity)
    {
        Emblems emblem = (Emblems)entity.Id;
        if (await repository.HasEmblem(emblem))
        {
            // TODO: load EmblemData.json and give correct entity
            return new(RewardGrantResult.Discarded);
        }

        repository.AddEmblem(emblem);
        return new(RewardGrantResult.Added);
    }
}
