using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Models.Requests;

// We only need to deserialize deviceAccount from the request. The rest of it is useless
public record LoginRequest(DeviceAccount? deviceAccount);
