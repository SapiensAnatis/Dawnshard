using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record AuthResponse(AuthResponseData data) : BaseResponse<AuthResponseData>;

[MessagePackObject(keyAsPropertyName: true)]
public record AuthResponseData(long viewer_id, string session_id, string nonce);