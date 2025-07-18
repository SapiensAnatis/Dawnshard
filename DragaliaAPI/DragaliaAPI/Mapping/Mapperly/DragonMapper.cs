using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class DragonMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DragonList.StatusPlusCount))]
    public static partial DragonList ToDragonList(this DbPlayerDragonData dbModel);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbPlayerDragonData.Owner))]
    public static partial DbPlayerDragonData MapToDbPlayerDragonData(
        this DragonList dragonList,
        long viewerId
    );
}
