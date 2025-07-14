using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class FortBuildMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial BuildList MapToBuildList(this DbFortBuild dbEntity);

    public static DbFortBuild MapToDbFortBuild(this BuildList buildList, long viewerId)
    {
        return new DbFortBuild()
        {
            ViewerId = viewerId,
            BuildId = (long)buildList.BuildId,
            PlantId = buildList.PlantId,
            Level = buildList.Level,
            PositionX = buildList.PositionX,
            PositionZ = buildList.PositionZ,
            BuildStartDate = buildList.BuildStartDate,
            BuildEndDate = buildList.BuildEndDate,
            IsNew = buildList.IsNew,
            LastIncomeDate = DateTimeOffset.UtcNow - buildList.LastIncomeTime,
        };
    }
}
