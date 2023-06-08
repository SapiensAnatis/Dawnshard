using DragaliaAPI.Photon.Dto.Game;
using NRediSearch;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Redis;

public interface IRedisService
{
    IDatabase Database { get; init; }
    Client SearchClient { get; init; }

    IAsyncEnumerable<Game> SearchGames(string searchString);
}
