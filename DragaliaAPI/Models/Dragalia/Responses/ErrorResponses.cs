namespace DragaliaAPI.Models.Dragalia.Responses
{
    public record ErrorData(ResultCode result_code);

    public record ServerErrorResponse : BaseResponse<ErrorData>
    {
        public override ErrorData data { get; init; } = new(ResultCode.ServerError);

        public ServerErrorResponse() : base(ResultCode.ServerError) { }
    }
}
