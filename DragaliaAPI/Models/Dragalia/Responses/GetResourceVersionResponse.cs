using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record GetResourceVersionResponse : BaseResponse<GetResourceVersionData>
{
    public override GetResourceVersionData data { get; init; } = new();
}

[MessagePackObject(keyAsPropertyName: true)]
public record GetResourceVersionData(string resource_version = "y2XM6giU6zz56wCm");