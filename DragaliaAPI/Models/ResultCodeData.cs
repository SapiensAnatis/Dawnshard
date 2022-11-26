using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(true)]
public record ResultCodeData(ResultCode result_code);
