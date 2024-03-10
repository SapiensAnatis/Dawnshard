using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class WeaponBodyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(DbWeaponBody.Ability2Level), nameof(WeaponBodyList.Ability2Levell))]
    public static partial WeaponBodyList ToWeaponBodyList(this DbWeaponBody dbEntity);
}
