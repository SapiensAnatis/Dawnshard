using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class TalismanMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial TalismanList ToTalismanList(this DbTalisman dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbTalisman.Owner))]
    public static partial DbTalisman MapToDbTalisman(this TalismanList talismanList, long viewerId);
}
