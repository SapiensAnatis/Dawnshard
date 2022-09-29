using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record GetDeployVersionResponse : BaseResponse<GetDeployVersionData>
{
    public override GetDeployVersionData data { get; init; } = new();
}

[MessagePackObject(keyAsPropertyName: true)]
public record GetDeployVersionData(string deploy_hash = "3bb2827ce9e6a66015ac2808112e3442740e862");