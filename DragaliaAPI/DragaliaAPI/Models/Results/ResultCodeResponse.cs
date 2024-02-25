using MessagePack;

namespace DragaliaAPI.Models.Results;

[MessagePackObject(true)]
public record ResultCodeResponse(
    [property: Key("result_code")] ResultCode ResultCode,
    [property: Key("message")] string? Message = null
);
