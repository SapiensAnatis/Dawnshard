using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class WeaponPassiveAbilityMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponPassiveAbilityList ToWeaponPassiveAbilityList(
        this DbWeaponPassiveAbility dbEntity
    );
}
