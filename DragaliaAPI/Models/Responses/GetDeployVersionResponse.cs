using DragaliaAPI.Models.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record GetDeployVersionResponse(GetDeployVersionData data)
    : BaseResponse<GetDeployVersionData>;

[MessagePackObject(keyAsPropertyName: true)]
public record GetDeployVersionData(string deploy_hash);

public static class GetDeployVersionStatic
{
    public static string DeployHash { get; } = "13bb2827ce9e6a66015ac2808112e3442740e862";
}
