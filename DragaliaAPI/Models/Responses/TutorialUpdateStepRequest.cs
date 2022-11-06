using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record TutorialUpdateStepRequest(int step);
