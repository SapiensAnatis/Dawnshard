using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class DragonReliabilityMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(
        nameof(DbPlayerDragonReliability.Level),
        nameof(DragonReliabilityList.ReliabilityLevel)
    )]
    [MapProperty(
        nameof(DbPlayerDragonReliability.Exp),
        nameof(DragonReliabilityList.ReliabilityTotalExp)
    )]
    public static partial DragonReliabilityList ToDragonReliabilityList(
        this DbPlayerDragonReliability dbEntity
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(
        nameof(DragonReliabilityList.ReliabilityLevel),
        nameof(DbPlayerDragonReliability.Level)
    )]
    [MapProperty(
        nameof(DragonReliabilityList.ReliabilityTotalExp),
        nameof(DbPlayerDragonReliability.Exp)
    )]
    [MapperIgnoreTarget(nameof(DbPlayerDragonReliability.Owner))]
    public static partial DbPlayerDragonReliability ToDbPlayerDragonReliability(
        this DragonReliabilityList dbEntity,
        long viewerId
    );
}
