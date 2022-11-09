using MessagePack;

namespace DragaliaAPI.Models.Responses.Base;

[MessagePackObject(keyAsPropertyName: true)]
public record DataHeaders(ResultCode result_code);

[MessagePackObject(keyAsPropertyName: true)]
public abstract record BaseResponse<TData> where TData : class
{
    public DataHeaders data_headers { get; init; }

    public abstract TData data { get; init; }

    public BaseResponse(ResultCode result_code = ResultCode.Success)
    {
        data_headers = new(result_code);
    }
}
