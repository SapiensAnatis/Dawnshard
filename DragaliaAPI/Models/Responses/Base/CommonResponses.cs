using MessagePack;

namespace DragaliaAPI.Models.Responses.Base;

[MessagePackObject(keyAsPropertyName: true)]
public record OkResponse : BaseResponse<DataHeaders>
{
    public override DataHeaders data { get; init; } = new(ResultCode.Success);
}

[MessagePackObject(keyAsPropertyName: true)]
public record ServerErrorResponse() : BaseResponse<DataHeaders>(ResultCode.ServerError)
{
    public override DataHeaders data { get; init; } = new(ResultCode.ServerError);
}
