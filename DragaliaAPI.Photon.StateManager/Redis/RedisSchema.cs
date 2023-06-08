using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Redis;

public static class RedisSchema
{
    public const string Prefix = "game";

    public static RedisKey Game(string gameName) => new($"{Prefix}:{gameName}");
}
