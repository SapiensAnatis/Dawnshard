using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class DragonMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DragonList.StatusPlusCount))]
    public static partial DragonList ToDragonList(this DbPlayerDragonData dbModel);
}
