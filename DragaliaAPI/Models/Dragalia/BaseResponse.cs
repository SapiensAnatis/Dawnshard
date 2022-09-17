using MessagePack;

namespace DragaliaAPI.Models.Dragalia
{
    [MessagePackObject(keyAsPropertyName: true)]
    public abstract record BaseResponse<TData> where TData : class
    {
        public DataHeaders data_headers { get; init; }

        public abstract TData data { get; init; }

        public BaseResponse(ResultCode result_code = ResultCode.Success)
        {
            this.data_headers = new(result_code);
        }

        public record DataHeaders(ResultCode result_code);
    }

    public enum ResultCode
    {
        Success = 1,
    }
}
