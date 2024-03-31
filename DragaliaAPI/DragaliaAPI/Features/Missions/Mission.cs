using System.Diagnostics;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public class Mission
{
    public MissionType Type { get; init; }
    public EntityTypes EntityType { get; init; }
    public int EntityId { get; init; }
    public int EntityQuantity { get; init; }
    public int CompleteValue { get; init; }
    public IMission MasterAssetMission { get; init; }

    public Mission()
    {
        MasterAssetMission = null!;
    }

    public static Mission From(MissionType type, int id)
    {
        return type switch
        {
            MissionType.Album => new Mission(MasterAsset.MissionAlbumData.Get(id), type),
            MissionType.Beginner => new Mission(MasterAsset.MissionBeginnerData.Get(id), type),
            MissionType.Daily => new Mission(MasterAsset.MissionDailyData.Get(id), type),
            MissionType.Drill => new Mission(MasterAsset.MissionDrillData.Get(id), type),
            MissionType.MainStory => new Mission(MasterAsset.MissionMainStoryData.Get(id), type),
            MissionType.MemoryEvent
                => new Mission(MasterAsset.MissionMemoryEventData.Get(id), type),
            MissionType.Normal => new Mission(MasterAsset.MissionNormalData.Get(id), type),
            MissionType.Period => new Mission(MasterAsset.MissionPeriodData.Get(id), type),
            MissionType.Special => new Mission(MasterAsset.MissionSpecialData.Get(id), type),
            MissionType.Invalid => throw new InvalidOperationException("Invalid MissionType"),
            _ => throw new UnreachableException("Unknown MissionType")
        };
    }

    public Mission(IMission mission, MissionType type)
    {
        Type = type;
        EntityType = mission.EntityType;
        EntityId = mission.EntityId;
        EntityQuantity = mission.EntityQuantity;
        CompleteValue = mission.CompleteValue;
        MasterAssetMission = mission;
    }
}
