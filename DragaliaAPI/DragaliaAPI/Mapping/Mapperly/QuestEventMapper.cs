using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class QuestEventMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestEventList ToQuestEventList(this DbQuestEvent dbEntity);
}
