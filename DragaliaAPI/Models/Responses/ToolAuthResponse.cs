using DragaliaAPI.Models.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record ToolAuthResponse(AuthResponseData data) : BaseResponse<AuthResponseData>;

[MessagePackObject(keyAsPropertyName: true)]
public record AuthResponseData(long viewer_id, string session_id, string nonce);
