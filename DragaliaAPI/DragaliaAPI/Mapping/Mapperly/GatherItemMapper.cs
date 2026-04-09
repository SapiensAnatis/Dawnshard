using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class GatherItemMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial GatherItemList MapToGatherItemList(this DbPlayerGatherItem dbEntity);
}
