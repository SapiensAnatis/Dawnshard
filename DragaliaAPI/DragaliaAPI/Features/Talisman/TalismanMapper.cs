using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Talisman;

[Mapper]
public static partial class TalismanMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial TalismanList ToTalismanList(this DbTalisman dbEntity);
}
