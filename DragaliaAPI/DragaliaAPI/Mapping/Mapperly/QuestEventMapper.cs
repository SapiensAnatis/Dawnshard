using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class QuestEventMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestEventList MapToQuestEventList(this DbQuestEvent dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbQuestEvent.Owner))]
    public static partial DbQuestEvent MapToDbQuestEvent(
        this QuestEventList questEventList,
        long viewerId
    );
}
