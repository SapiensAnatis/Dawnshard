using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class StoryMapper
{
    public static UnitStoryList MapToUnitStoryList(this DbPlayerStoryState dbEntity) =>
        new() { UnitStoryId = dbEntity.StoryId, IsRead = dbEntity.State == StoryState.Read };

    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapProperty(nameof(dbEntity.StoryId), nameof(QuestStoryList.QuestStoryId))]
    public static partial QuestStoryList MapToQuestStoryList(this DbPlayerStoryState dbEntity);

    public static CastleStoryList MapToCastleStoryList(this DbPlayerStoryState dbEntity) =>
        new() { CastleStoryId = dbEntity.StoryId, IsRead = dbEntity.State == StoryState.Read };

    public static DmodeStoryList MapToDmodeStoryList(this DbPlayerStoryState dbEntity) =>
        new() { DmodeStoryId = dbEntity.StoryId, IsRead = dbEntity.State == StoryState.Read };

    public static DbPlayerStoryState MapToDbPlayerStoryState(
        this QuestStoryList questStoryList,
        long viewerId
    )
    {
        return new()
        {
            ViewerId = viewerId,
            StoryId = questStoryList.QuestStoryId,
            StoryType = StoryTypes.Quest,
            State = questStoryList.State,
        };
    }

    public static DbPlayerStoryState MapToDbPlayerStoryState(
        this CastleStoryList castleStoryList,
        long viewerId
    )
    {
        return new()
        {
            ViewerId = viewerId,
            StoryId = castleStoryList.CastleStoryId,
            StoryType = StoryTypes.Castle,
            State = castleStoryList.IsRead ? StoryState.Read : StoryState.Unlocked,
        };
    }

    public static DbPlayerStoryState MapToDbPlayerStoryState(
        this UnitStoryList unitStoryList,
        long viewerId
    )
    {
        // Normally this would do a MasterAsset lookup to get the correct story type
        // based on the target chara/dragon id. However, this is slow, and all dragon ids start
        // at 200000000, meaning that we can just check for that here and decide based on that
        StoryTypes storyType =
            unitStoryList.UnitStoryId > 200000000 ? StoryTypes.Dragon : StoryTypes.Chara;

        return new()
        {
            ViewerId = viewerId,
            StoryId = unitStoryList.UnitStoryId,
            StoryType = storyType,
            State = unitStoryList.IsRead ? StoryState.Read : StoryState.Unlocked,
        };
    }
}
