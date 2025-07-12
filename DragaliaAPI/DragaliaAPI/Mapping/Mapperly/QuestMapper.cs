using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class QuestMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestList MapToQuestList(this DbQuest dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbQuest.Owner))]
    public static partial DbQuest MapToDbQuest(this QuestList questList, long viewerId);
}
