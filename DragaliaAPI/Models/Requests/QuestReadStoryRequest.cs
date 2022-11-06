using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(true)]
public record QuestReadStoryRequest(int quest_story_id);
