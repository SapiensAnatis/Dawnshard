namespace DragaliaAPI.Models.Dragalia.Responses
{
    public record VerifyJwsResponse : BaseResponse<VerifyJwsData>
    {
        public override VerifyJwsData data { get; init; } = new();
    }

    public record VerifyJwsData();
}
