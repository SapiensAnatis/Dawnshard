using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Models.Options;

public class ItemSummonOdds
{
    public List<ItemSummonOddsEntry> Odds { get; set; } = new();
}

public class ItemSummonOddsEntry 
{
    public EntityTypes Type { get; set; }
    public int Id { get; set; }
    public int Quantity { get; set; }
    public double Rate { get; set; }
}