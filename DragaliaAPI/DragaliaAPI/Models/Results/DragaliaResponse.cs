using System.Text.Json.Serialization;
using MessagePack;

namespace DragaliaAPI.Models.Results;

public abstract class DragaliaResponse
{
    [Key("data_headers")]
    public DataHeaders DataHeaders { get; }

    protected DragaliaResponse(DataHeaders dataHeaders)
    {
        this.DataHeaders = dataHeaders;
    }
}

[MessagePackObject]
public class DragaliaResponse<TData> : DragaliaResponse
    where TData : class
{
    [Key("data")]
    public TData Data { get; }

    public DragaliaResponse(TData data, ResultCode resultCode = ResultCode.Success)
        : this(data, new DataHeaders(resultCode)) { }

    [JsonConstructor]
    [SerializationConstructor]
    public DragaliaResponse(TData data, DataHeaders dataHeaders)
        : base(dataHeaders)
    {
        this.Data = data;
    }
}
