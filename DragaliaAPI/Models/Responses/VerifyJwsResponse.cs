using DragaliaAPI.Models.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record VerifyJwsResponse(VerifyJwsData data) : BaseResponse<VerifyJwsData>;

[MessagePackObject(keyAsPropertyName: true)]
public record VerifyJwsData();
