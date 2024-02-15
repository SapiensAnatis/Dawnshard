using System.Collections.Immutable;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class EmblemRewardHandler(IEmblemRepository repository) : IRewardHandler
{
    public ImmutableArray<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.Title);

    public async Task<GrantReturn> Grant(Entity entity)
    {
        Emblems emblem = (Emblems)entity.Id;

        if (!Enum.IsDefined(emblem))
            throw new ArgumentException("Entity ID is not a valid emblem", nameof(entity));

        if (await repository.HasEmblem(emblem))
        {
            // TODO: load EmblemData.json and give correct entity
            return new(RewardGrantResult.Discarded);
        }

        repository.AddEmblem(emblem);
        return new(RewardGrantResult.Added);
    }
}
