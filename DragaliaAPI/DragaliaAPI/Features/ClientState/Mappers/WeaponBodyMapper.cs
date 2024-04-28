using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class WeaponBodyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponBodyList ToWeaponBodyList(this DbWeaponBody dbEntity);
}
