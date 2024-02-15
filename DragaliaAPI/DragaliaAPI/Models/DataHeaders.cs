using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(keyAsPropertyName: true)]
public record DataHeaders(ResultCode result_code);
