using DragaliaAPI.Models.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(keyAsPropertyName: true)]
public record ServiceStatusResponse(ServiceStatusData data) : BaseResponse<ServiceStatusData>;

[MessagePackObject(keyAsPropertyName: true)]
public record ServiceStatusData(int service_status);
