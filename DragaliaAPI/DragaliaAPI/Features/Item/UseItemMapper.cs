using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Item;

[Mapper]
public static partial class UseItemMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial ItemList ToItemList(this DbPlayerUseItem dbEntity);
}
