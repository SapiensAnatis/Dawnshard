using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class QuestEventMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestEventList ToQuestEventList(this DbQuestEvent dbEntity);
}
