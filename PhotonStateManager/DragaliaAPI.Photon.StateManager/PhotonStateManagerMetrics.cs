using System.Diagnostics;
using System.Diagnostics.Metrics;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.StateManager.Models;
using Redis.OM.Contracts;

namespace DragaliaAPI.Photon.StateManager;

public class PhotonStateManagerMetrics
{
    public static string MeterName => "PhotonStateManager";

    private readonly IRedisConnectionProvider connectionProvider;

    public PhotonStateManagerMetrics(
        IMeterFactory meterFactory,
        IRedisConnectionProvider connectionProvider
    )
    {
        this.connectionProvider = connectionProvider;
        Meter meter = meterFactory.Create(MeterName);

        meter.CreateObservableUpDownCounter("photon_state_manager.rooms.count", this.GetRoomCount);
    }

    private IEnumerable<Measurement<int>> GetRoomCount()
    {
        List<RedisGame> games = this.connectionProvider.RedisCollection<RedisGame>().ToList();

        List<Measurement<int>> measurements = games
            .GroupBy(
                RoomTags.Create,
                (tags, rooms) => new Measurement<int>(rooms.Count(), tags.ToTagList())
            )
            .DefaultIfEmpty(new Measurement<int>(0))
            .ToList();

        return measurements;
    }
}

file record RoomTags(MatchingTypes MatchingType, int PlayerCount, bool Visible)
{
    public static RoomTags Create(RedisGame game) =>
        new(game.MatchingType, game.Players.Count, game.Visible);

    public TagList ToTagList() =>
        new()
        {
            new("room.matching_type", this.MatchingType.ToString()),
            new("room.player_count", this.PlayerCount),
            new("room.visible", this.Visible),
        };
}
