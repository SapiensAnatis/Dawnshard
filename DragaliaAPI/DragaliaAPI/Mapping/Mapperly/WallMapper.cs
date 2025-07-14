using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class WallMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestWallList MapToQuestWallList(this DbPlayerQuestWall dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbPlayerQuestWall.Owner))]
    public static partial DbPlayerQuestWall MapToDbPlayerQuestWall(
        this QuestWallList questTreasureList,
        long viewerId
    );
}
