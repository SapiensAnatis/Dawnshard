using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class QuestMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestList ToQuestList(this DbQuest dbEntity);
}
