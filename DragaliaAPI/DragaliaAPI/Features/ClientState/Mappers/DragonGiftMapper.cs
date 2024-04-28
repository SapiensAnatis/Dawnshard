using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class DragonGiftMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial DragonGiftList ToDragonGift(this DbPlayerDragonGift dbEntity);
}
