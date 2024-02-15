using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Models.Options;

public class ItemSummonConfig
{
    public required List<ItemSummonOddsEntry> Odds { get; set; }
}

public class ItemSummonOddsEntry
{
    public EntityTypes Type { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int Rate { get; set; }
}
