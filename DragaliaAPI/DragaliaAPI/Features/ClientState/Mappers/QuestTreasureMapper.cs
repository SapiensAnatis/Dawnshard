using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class QuestTreasureMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestTreasureList ToQuestTreasureList(this DbQuestTreasureList dbEntity);
}
