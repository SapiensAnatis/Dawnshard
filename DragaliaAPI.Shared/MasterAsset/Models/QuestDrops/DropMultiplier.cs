namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public abstract record DropMultiplier
{
    public double ManaMultiplier { get; init; } = 1;
    public double MaterialMultiplier { get; init; } = 1;
    public double RupieMultiplier { get; init; } = 1;
}
