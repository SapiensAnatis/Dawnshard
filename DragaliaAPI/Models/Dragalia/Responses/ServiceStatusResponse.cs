using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record ServiceStatusResponse : BaseResponse<ServiceStatusData>
{
    public override ServiceStatusData data { get; init; }

    [SerializationConstructor]
    public ServiceStatusResponse()
    {
        data = new(1);
    }

}

[MessagePackObject(keyAsPropertyName: true)]
public record ServiceStatusData(int service_status);