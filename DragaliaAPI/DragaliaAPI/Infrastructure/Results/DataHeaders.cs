using MessagePack;

namespace DragaliaAPI.Infrastructure.Results;

[MessagePackObject]
public record DataHeaders([property: Key("result_code")] ResultCode ResultCode);
