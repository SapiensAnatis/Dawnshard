using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Infrastructure.Mapping.Mapperly;

[Mapper]
public static partial class QuestEventMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestEventList ToQuestEventList(this DbQuestEvent dbEntity);
}
