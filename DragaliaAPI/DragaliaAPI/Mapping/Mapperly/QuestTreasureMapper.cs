using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class QuestTreasureMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestTreasureList ToQuestTreasureList(this DbQuestTreasureList dbEntity);
}
