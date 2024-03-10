using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class StoryMapper
{
    public static UnitStoryList ToUnitStoryList(this DbPlayerStoryState dbEntity) =>
        new() { UnitStoryId = dbEntity.StoryId, IsRead = dbEntity.State == StoryState.Read, };

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(dbEntity.StoryId), nameof(QuestStoryList.QuestStoryId))]
    public static partial QuestStoryList ToQuestStoryList(this DbPlayerStoryState dbEntity);

    public static CastleStoryList ToCastleStoryList(this DbPlayerStoryState dbEntity) =>
        new() { CastleStoryId = dbEntity.StoryId, IsRead = dbEntity.State == StoryState.Read, };

    public static DmodeStoryList ToDmodeStoryList(this DbPlayerStoryState dbEntity) =>
        new() { DmodeStoryId = dbEntity.StoryId, IsRead = dbEntity.State == StoryState.Read, };
}
