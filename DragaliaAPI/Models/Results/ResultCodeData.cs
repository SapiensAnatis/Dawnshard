using MessagePack;

namespace DragaliaAPI.Models.Results;

[MessagePackObject(true)]
public record ResultCodeData(ResultCode result_code, string? message = null);
