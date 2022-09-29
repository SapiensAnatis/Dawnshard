using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record VerifyJwsResponse : BaseResponse<VerifyJwsData>
{
    public override VerifyJwsData data { get; init; } = new();
}

[MessagePackObject(keyAsPropertyName: true)]
public record VerifyJwsData();