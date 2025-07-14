using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class WeaponSkinMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponSkinList MapToWeaponSkinList(this DbWeaponSkin dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbWeaponSkin.Owner))]
    public static partial DbWeaponSkin MapToDbWeaponSkin(
        this WeaponSkinList weaponSkinList,
        long viewerId
    );
}
