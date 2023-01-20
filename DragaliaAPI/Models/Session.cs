namespace DragaliaAPI.Services;

public record Session(string SessionId, string IdToken, string DeviceAccountId, long ViewerId);
