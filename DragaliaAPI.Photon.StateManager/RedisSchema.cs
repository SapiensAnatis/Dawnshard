using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager;

public static class RedisSchema
{
    public static RedisKey GameList() => new("game:list");

    public static RedisKey GameInfo(string gameName) => new($"game:{gameName}:info");

    public static RedisKey GamePlayers(string gameName) => new($"game:{gameName}:players");
}
