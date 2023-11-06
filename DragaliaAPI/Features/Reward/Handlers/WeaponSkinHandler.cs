using System.Collections.Immutable;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

public class WeaponSkinHandler(IWeaponRepository weaponRepository) : IRewardHandler
{
    public ImmutableArray<EntityTypes> SupportedTypes { get; } =
        ImmutableArray.Create(EntityTypes.WeaponSkin);

    public async Task<GrantReturn> Grant(Entity entity)
    {
        // TODO: lookup duplicate reward in masterasset
        if (await weaponRepository.WeaponSkins.AnyAsync(x => x.WeaponSkinId == entity.Id))
            return new GrantReturn(RewardGrantResult.Discarded);

        await weaponRepository.AddSkin(entity.Id);
        return new GrantReturn(RewardGrantResult.Added);
    }
}
