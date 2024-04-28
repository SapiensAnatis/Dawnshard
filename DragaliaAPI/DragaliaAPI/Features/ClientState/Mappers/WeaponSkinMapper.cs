using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class WeaponSkinMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponSkinList ToWeaponSkinList(this DbWeaponSkin dbEntity);
}
