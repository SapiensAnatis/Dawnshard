using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record ServiceStatusResponse(ServiceStatusData data) : BaseResponse<ServiceStatusData>;

[MessagePackObject(keyAsPropertyName: true)]
public record ServiceStatusData(int service_status);