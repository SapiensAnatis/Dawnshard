using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record NamechangeResponse(NamechangeData data) : BaseResponse<NamechangeData>;

[MessagePackObject(keyAsPropertyName: true)]
public record NamechangeData(string checked_name);
