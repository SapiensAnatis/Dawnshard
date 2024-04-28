using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Target)]
public static partial class WeaponBodyMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial WeaponBodyList ToWeaponBodyList(this DbWeaponBody dbEntity);
}
