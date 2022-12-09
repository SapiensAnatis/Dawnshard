using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(keyAsPropertyName: true)]
public class DragaliaResponse<TData> where TData : class
{
    public DataHeaders data_headers { get; init; }

    public TData data { get; init; }

    public DragaliaResponse(TData data, ResultCode result_code = ResultCode.SUCCESS)
    {
        this.data = data;
        this.data_headers = new(result_code);
    }

    [SerializationConstructor]
    public DragaliaResponse(DataHeaders data_headers, TData data)
    {
        this.data_headers = data_headers;
        this.data = data;
    }
}
