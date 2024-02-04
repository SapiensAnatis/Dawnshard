namespace DragaliaAPI.Models.Options;

public class RedisOptions
{
    public string Hostname { get; set; } = "redis";

    public int Port { get; set; } = 6379;

    public string? Password { get; set; }
}
