using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Savefile.Mappers;

[Mapper]
public static partial class QuestMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    public static partial QuestList ToQuestList(this DbQuest dbEntity);

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(dbEntity.StoryId), nameof(QuestStoryList.QuestStoryId))]
    public static partial QuestStoryList ToQuestStoryList(this DbPlayerStoryState dbEntity);
}
