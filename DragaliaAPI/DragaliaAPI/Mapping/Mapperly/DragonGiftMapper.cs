using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class DragonGiftMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial DragonGiftList ToDragonGift(this DbPlayerDragonGift dbEntity);
}
