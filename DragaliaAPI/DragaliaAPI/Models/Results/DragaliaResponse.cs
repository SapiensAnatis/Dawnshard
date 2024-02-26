using System.Text.Json.Serialization;
using MessagePack;

namespace DragaliaAPI.Models.Results;

[MessagePackObject]
public class DragaliaResponse<TData>
    where TData : class
{
    [Key("data_headers")]
    public DataHeaders DataHeaders { get; init; }

    [Key("data")]
    public TData Data { get; init; }

    public DragaliaResponse(TData data, ResultCode resultCode = ResultCode.Success)
    {
        this.Data = data;
        this.DataHeaders = new(resultCode);
    }

    [JsonConstructor]
    [SerializationConstructor]
    public DragaliaResponse(DataHeaders dataHeaders, TData data)
    {
        this.DataHeaders = dataHeaders;
        this.Data = data;
    }
}
