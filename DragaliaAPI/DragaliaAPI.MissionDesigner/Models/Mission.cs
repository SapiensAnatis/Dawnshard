namespace DragaliaAPI.MissionDesigner.Models;

public abstract class Mission
{
    private MissionType? type;

    protected int Id => int.Parse($"{this.MissionId}{(int)this.Type:00}");

    public required int MissionId { get; init; }

    public int? ProgressionGroupId { get; init; }

    public MissionType Type
    {
        get => this.type ?? throw new InvalidOperationException("Type was not initialized");
        set => this.type = value;
    }

    protected abstract MissionCompleteType CompleteType { get; }

    protected virtual int? Parameter => null;

    protected virtual int? Parameter2 => null;

    protected virtual int? Parameter3 => null;

    protected virtual int? Parameter4 => null;

    protected virtual bool UseTotalValue => false;

    public MissionProgressionInfo ToMissionProgressionInfo() =>
        new MissionProgressionInfo(
            this.Id,
            this.Type,
            this.MissionId,
            this.CompleteType,
            this.UseTotalValue,
            this.ProgressionGroupId,
            [],
            this.Parameter,
            this.Parameter2,
            this.Parameter3,
            this.Parameter4
        );
}
