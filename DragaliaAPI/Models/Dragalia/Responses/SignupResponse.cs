using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record SignupResponse : BaseResponse<SignupData>
{
    public override SignupData data { get; init; }

    public SignupResponse(SignupData data) 
    {
        this.data = data;
    }
}

[MessagePackObject(keyAsPropertyName: true)]
public record SignupData(long viewer_id);

