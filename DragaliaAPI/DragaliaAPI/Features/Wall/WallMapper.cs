using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Wall;

[Mapper]
public static partial class WallMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestWallList ToQuestWallList(this DbPlayerQuestWall dbEntity);
}
