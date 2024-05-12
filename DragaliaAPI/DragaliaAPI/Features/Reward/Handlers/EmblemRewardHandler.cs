using System.Collections.Immutable;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class EmblemRewardHandler(IEmblemRepository repository) : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.Title);

    public async Task<GrantReturn> Grant(Entity entity)
    {
        Emblems emblem = (Emblems)entity.Id;

        if (!Enum.IsDefined(emblem))
            throw new ArgumentException("Entity ID is not a valid emblem", nameof(entity));

        if (!await repository.HasEmblem(emblem))
        {
            repository.AddEmblem(emblem);
            return GrantReturn.Added();
        }

        if (
            MasterAsset.EmblemData.TryGetValue(emblem, out EmblemData? data)
            && data is { DuplicateEntityType: not 0, DuplicateEntityQuantity: not 0 }
        )
        {
            Entity convertedEntity =
                new(data.DuplicateEntityType, data.DuplicateEntityId, data.DuplicateEntityQuantity);

            return GrantReturn.Converted(convertedEntity);
        }

        return GrantReturn.Discarded();
    }
}
