using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(true)]
public record QuestReadStoryRequest(int questStoryId);
