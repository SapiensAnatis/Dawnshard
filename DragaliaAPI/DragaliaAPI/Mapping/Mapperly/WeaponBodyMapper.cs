using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class WeaponBodyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponBodyList MapToWeaponBodyList(this DbWeaponBody dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbWeaponBody.Owner))]
    public static partial DbWeaponBody MapToDbWeaponBody(
        this WeaponBodyList weaponBodyList,
        long viewerId
    );
}
