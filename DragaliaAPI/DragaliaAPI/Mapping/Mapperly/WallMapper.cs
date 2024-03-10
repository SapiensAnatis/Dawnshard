using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class WallMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestWallList ToQuestWallList(this DbPlayerQuestWall dbEntity);
}
