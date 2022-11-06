using MessagePack;

namespace DragaliaAPI.Models.Components;

[MessagePackObject(true)]
public record QuestStory(int quest_story_id, int state);
