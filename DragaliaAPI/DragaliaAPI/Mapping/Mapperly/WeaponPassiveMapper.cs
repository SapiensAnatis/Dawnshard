using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class WeaponPassiveAbilityMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponPassiveAbilityList MapToWeaponPassiveAbilityList(
        this DbWeaponPassiveAbility dbEntity
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbWeaponPassiveAbility.Owner))]
    public static partial DbWeaponPassiveAbility MapToDbWeaponPassiveAbility(
        this WeaponPassiveAbilityList weaponPassiveAbilityList,
        long viewerId
    );
}
