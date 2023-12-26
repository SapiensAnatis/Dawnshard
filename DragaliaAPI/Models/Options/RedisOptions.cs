namespace DragaliaAPI.Models.Options;

public class RedisOptions
{
    public int SessionExpiryTimeMinutes { get; set; }
    public int DungeonExpiryTimeMinutes { get; set; }

    public int AutoRepeatExpiryTimeMinutes { get; set; }
}
