using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class UseItemMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial ItemList ToItemList(this DbPlayerUseItem dbEntity);
}
