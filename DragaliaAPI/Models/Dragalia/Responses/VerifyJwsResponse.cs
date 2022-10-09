using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record VerifyJwsResponse(VerifyJwsData data) : BaseResponse<VerifyJwsData>;

[MessagePackObject(keyAsPropertyName: true)]
public record VerifyJwsData();
