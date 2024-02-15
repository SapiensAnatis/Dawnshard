using System.Collections.Immutable;
using DragaliaAPI.Database.Repositories;
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
    public ImmutableArray<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.Wyrmprint);

    public async Task<GrantReturn> Grant(Entity entity)
    {
        AbilityCrests crest = (AbilityCrests)entity.Id;
        if (!Enum.IsDefined(crest))
            throw new ArgumentException("Entity ID was not a valid ability crest", nameof(entity));

        if (await abilityCrestRepository.FindAsync(crest) is not null)
        {
            AbilityCrest crestData = MasterAsset.AbilityCrest[crest];

            Entity duplicateEntity =
                new(
                    crestData.DuplicateEntityType,
                    Id: (int)crestData.DuplicateEntityId,
                    Quantity: crestData.DuplicateEntityQuantity * entity.Quantity
                );

            logger.LogDebug(
                "Converted ability crest entity: {@entity} to {@duplicateEntity}.",
                entity,
                duplicateEntity
            );

            return new(RewardGrantResult.Converted, duplicateEntity);
        }

        logger.LogDebug("Granted new ability crest entity: {@entity}", entity);
        await abilityCrestRepository.Add(
            crest,
            entity.LimitBreakCount,
            entity.BuildupCount,
            entity.EquipableCount
        );

        return new(RewardGrantResult.Added);
    }
}
