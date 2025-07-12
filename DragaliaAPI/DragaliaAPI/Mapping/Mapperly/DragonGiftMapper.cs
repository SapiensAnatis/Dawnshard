using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class DragonGiftMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial DragonGiftList MapToDragonGiftList(this DbPlayerDragonGift dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbPlayerDragonGift.Owner))]
    public static partial DbPlayerDragonGift MapToDbPlayerDragonGift(
        this DragonGiftList dragonGiftList,
        long viewerId
    );
}
