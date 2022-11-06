using DragaliaAPI.Models.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record SignupResponse(SignupData data) : BaseResponse<SignupData>;

[MessagePackObject(keyAsPropertyName: true)]
public record SignupData(long viewer_id);
