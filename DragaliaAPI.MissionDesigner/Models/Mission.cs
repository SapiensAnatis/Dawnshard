using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public abstract record Mission
{
    public int Id => int.Parse($"{this.MissionId}{(int)this.Type:00}");

    public required int MissionId { get; init; }

    public required MissionType Type { get; init; }

    public abstract MissionCompleteType CompleteType { get; }

    public abstract MissionProgressionInfo ToMissionProgressionInfo();
}
