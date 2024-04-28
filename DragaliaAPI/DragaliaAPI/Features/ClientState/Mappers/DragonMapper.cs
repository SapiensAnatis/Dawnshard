using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class DragonMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DragonList.StatusPlusCount))]
    public static partial DragonList ToDragonList(this DbPlayerDragonData dbModel);
}
