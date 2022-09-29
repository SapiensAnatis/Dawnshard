namespace DragaliaAPI.Models.Dragalia.Responses;

public record OkResponse : BaseResponse<DataHeaders>
{
    public override DataHeaders data { get; init; } = new(ResultCode.Success);
}

public record ServerErrorResponse() : BaseResponse<DataHeaders>(ResultCode.ServerError)
{
    public override DataHeaders data { get; init; } = new(ResultCode.ServerError);
}