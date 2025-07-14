using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class QuestTreasureMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestTreasureList MapToQuestTreasureList(
        this DbQuestTreasureList dbEntity
    );

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(DbQuestTreasureList.Owner))]
    public static partial DbQuestTreasureList MapToDbQuestTreasureList(
        this QuestTreasureList questTreasureList,
        long viewerId
    );
}
