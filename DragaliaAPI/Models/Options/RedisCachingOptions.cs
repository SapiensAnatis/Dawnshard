namespace DragaliaAPI.Models.Options;

public class RedisCachingOptions
{
    public int SessionExpiryTimeMinutes { get; set; } = 60;

    public int DungeonExpiryTimeMinutes { get; set; } = 360;

    public int AutoRepeatExpiryTimeMinutes { get; set; } = 1800;
}
