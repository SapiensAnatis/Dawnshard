using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.MissionDesigner.Models;

public abstract class Mission
{
    private MissionType? type;

    protected int Id => int.Parse($"{this.MissionId}{(int)this.Type:00}");

    public required int MissionId { get; init; }

    public MissionType Type
    {
        get => this.type ?? throw new InvalidOperationException("Type was not initialized");
        set => this.type = value;
    }

    public abstract MissionCompleteType CompleteType { get; }

    public abstract MissionProgressionInfo ToMissionProgressionInfo();
}
