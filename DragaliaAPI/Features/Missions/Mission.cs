using System.Diagnostics;
using DragaliaAPI.Database.Utils;
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
            MissionType.Album => new Mission(MasterAsset.AlbumMission.Get(id), type),
            MissionType.Beginner => new Mission(MasterAsset.BeginnerMission.Get(id), type),
            MissionType.Daily => new Mission(MasterAsset.DailyMission.Get(id), type),
            MissionType.Drill => new Mission(MasterAsset.DrillMission.Get(id), type),
            MissionType.MainStory => new Mission(MasterAsset.MainStoryMission.Get(id), type),
            MissionType.MemoryEvent => new Mission(MasterAsset.MemoryEventMission.Get(id), type),
            MissionType.Normal => new Mission(MasterAsset.NormalMission.Get(id), type),
            MissionType.Period => new Mission(MasterAsset.PeriodMission.Get(id), type),
            MissionType.Special => new Mission(MasterAsset.SpecialMission.Get(id), type),
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
