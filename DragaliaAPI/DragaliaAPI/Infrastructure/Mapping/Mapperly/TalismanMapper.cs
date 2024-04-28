using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class TalismanMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial TalismanList ToTalismanList(this DbTalisman dbEntity);
}
