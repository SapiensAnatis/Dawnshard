namespace DragaliaAPI.Photon.StateManager.Models;

public class RedisOptions
{
    public required string Hostname { get; set; }

    public int Port { get; set; }

    public string? Password { get; set; }
}
