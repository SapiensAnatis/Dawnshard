using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager;

public static class RedisSchema
{
    public static RedisKey GameList() => new("game:list");

    public static RedisKey Game(string gameName) => new($"game:{gameName}");

    public static RedisKey GamePlayers(string gameName) => new($"game:{gameName}:players");
}
