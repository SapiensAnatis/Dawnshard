using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record RedoableSummonGetDataResponse(RedoableSummonGetDataData data)
    : BaseResponse<RedoableSummonGetDataData>;

[MessagePackObject(true)]
public record RedoableSummonGetDataData();
