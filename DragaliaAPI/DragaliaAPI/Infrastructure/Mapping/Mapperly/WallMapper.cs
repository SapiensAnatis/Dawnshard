using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class WallMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestWallList ToQuestWallList(this DbPlayerQuestWall dbEntity);
}
