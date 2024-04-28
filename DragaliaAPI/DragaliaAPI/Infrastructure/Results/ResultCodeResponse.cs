using MessagePack;

namespace DragaliaAPI.Infrastructure.Results;

[MessagePackObject]
public record ResultCodeResponse(
    [property: Key("result_code")] ResultCode ResultCode,
    [property: Key("message")] string? Message = null
);
