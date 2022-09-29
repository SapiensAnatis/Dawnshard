using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses
{
    [MessagePackObject(keyAsPropertyName: true)]
    public record AuthResponse : BaseResponse<AuthResponseData>
    {
        public override AuthResponseData data { get; init; }

        public AuthResponse(long viewer_id, string session_id)
        {
            this.data = new(viewer_id, session_id, "placeholder");
        }

        [SerializationConstructor]
        public AuthResponse()
        {
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public record AuthResponseData(long viewer_id, string session_id, string nonce);
}
