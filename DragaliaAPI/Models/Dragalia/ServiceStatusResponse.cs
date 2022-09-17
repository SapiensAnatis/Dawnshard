using MessagePack;

namespace DragaliaAPI.Models.Dragalia
{
    [MessagePackObject(keyAsPropertyName: true)]
    public record ServiceStatusResponse : BaseResponse<ServiceStatusData>
    {
        public override ServiceStatusData data { get; init; }

        [SerializationConstructor]
        public ServiceStatusResponse()
        {
            this.data = new(1);
        }

    }

    [MessagePackObject(keyAsPropertyName: true)]
    public record ServiceStatusData(int service_status);
}
