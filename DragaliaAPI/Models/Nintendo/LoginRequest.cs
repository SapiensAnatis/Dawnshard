namespace DragaliaAPI.Models.Nintendo;

// We only need to deserialize deviceAccount from the request. The rest of it is useless
[Obsolete("From pre-BaaS login flow")]
public record LoginRequest(DeviceAccount? deviceAccount);
