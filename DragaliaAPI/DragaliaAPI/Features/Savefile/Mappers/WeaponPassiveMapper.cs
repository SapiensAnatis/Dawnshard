using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class WeaponPassiveAbilityMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponPassiveAbilityList ToWeaponPassiveAbilityList(
        this DbWeaponPassiveAbility dbEntity
    );
}
