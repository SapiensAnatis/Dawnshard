using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject]
public record DataHeaders([property: Key("result_code")] ResultCode ResultCode);
