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
        int publicRooms = this
            .connectionProvider.RedisCollection<RedisGame>()
            .Count(x => x.MatchingType == MatchingTypes.Anyone);

        int privateRooms = this
            .connectionProvider.RedisCollection<RedisGame>()
            .Count(x => x.MatchingType != MatchingTypes.Anyone);

        return
        [
            new Measurement<int>(publicRooms, [new("room.public", true)]),
            new Measurement<int>(privateRooms, [new("room.public", false)]),
        ];
    }
}
