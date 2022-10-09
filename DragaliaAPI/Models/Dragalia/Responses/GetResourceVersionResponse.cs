using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record GetResourceVersionResponse(GetResourceVersionData data)
    : BaseResponse<GetResourceVersionData>;

[MessagePackObject(keyAsPropertyName: true)]
public record GetResourceVersionData(string resource_version);

public static class GetResourceVersionStatic
{
    public static string ResourceVersion { get; } = "y2XM6giU6zz56wCm";
}
