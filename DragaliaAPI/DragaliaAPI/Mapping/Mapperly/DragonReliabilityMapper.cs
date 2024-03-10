using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class DragonReliabilityMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(DbPlayerDragonData.Level), nameof(DragonReliabilityList.ReliabilityLevel))]
    [MapProperty(nameof(DbPlayerDragonData.Exp), nameof(DragonReliabilityList.ReliabilityTotalExp))]
    public static partial DragonReliabilityList ToDragonReliabilityList(
        this DbPlayerDragonReliability dbEntity
    );
}
