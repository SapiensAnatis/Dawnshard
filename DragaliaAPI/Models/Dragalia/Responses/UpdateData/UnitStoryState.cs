using DragaliaAPI.Models.Database.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record UnitStoryState(long unit_story_Id, bool isRead);

public static class UnitStoryStateFactory
{
    public static UnitStoryState Create(long storyId, bool isStoryRead)
    {
        return new UnitStoryState(unit_story_Id: storyId, isRead: isStoryRead);
    }
}
