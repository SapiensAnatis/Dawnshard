namespace DragaliaAPI.Models;

public record Session(string SessionId, string IdToken, string DeviceAccountId, long ViewerId);
