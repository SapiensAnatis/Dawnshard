using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record SignupResponse(SignupData data) : BaseResponse<SignupData>;

[MessagePackObject(keyAsPropertyName: true)]
public record SignupData(long viewer_id);

