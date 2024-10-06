using System.Collections.Immutable;
using DragaliaAPI.Features.AbilityCrests;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using JetBrains.Annotations;

namespace DragaliaAPI.Features.Reward.Handlers;

[UsedImplicitly]
public class AbilityCrestRewardHandler(
    IAbilityCrestRepository abilityCrestRepository,
    ILogger<AbilityCrestRewardHandler> logger
) : IRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.Wyrmprint);

    public async Task<GrantReturn> Grant(Entity entity)
    {
        AbilityCrestId crest = (AbilityCrestId)entity.Id;

        if (!MasterAsset.AbilityCrest.ContainsKey(crest))
        {
            throw new ArgumentException("Entity ID was not a valid ability crest", nameof(entity));
        }

        if (await abilityCrestRepository.FindAsync(crest) is not null)
        {
            AbilityCrest crestData = MasterAsset.AbilityCrest[crest];

            Entity duplicateEntity =
                new(
                    crestData.DuplicateEntityType,
                    Id: (int)crestData.DuplicateEntityId,
                    Quantity: crestData.DuplicateEntityQuantity * entity.Quantity
                );

            logger.LogTrace(
                "Converted ability crest entity: {@Entity} to {@DuplicateEntity}.",
                entity,
                duplicateEntity
            );

            return new(RewardGrantResult.Converted, duplicateEntity);
        }

        logger.LogTrace("Granted new ability crest entity: {@Entity}", entity);

        await abilityCrestRepository.Add(
            crest,
            entity.LimitBreakCount,
            entity.BuildupCount,
            entity.EquipableCount
        );

        return new(RewardGrantResult.Added);
    }
}
