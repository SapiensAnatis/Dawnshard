using MessagePack;

namespace DragaliaAPI.Models.Dragalia
{
    [MessagePackObject(keyAsPropertyName: true)]
    public record DataHeaders(ResultCode result_code);

    [MessagePackObject(keyAsPropertyName: true)]
    public abstract record BaseResponse<TData> where TData : class
    {
        public DataHeaders data_headers { get; init; }

        public abstract TData data { get; init; }

        public BaseResponse(ResultCode result_code = ResultCode.Success)
        {
            this.data_headers = new(result_code);
        }
    }

    public enum ResultCode
    {
        Success = 1,
		Maintenance = 101,
		ServerError = 102,
		SessionError = 201,
		AccountBlockError = 203,
		ApiVersionError = 204,
		ProcessedError = 213,
		MaintenanceFrom = 2000,
		MaintenanceTo = 2999
    }

}
