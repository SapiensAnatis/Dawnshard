using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class IngameResultData
{
    [Key("reward_record")]
    public RewardRecord RewardRecord { get; set; } = new();

    [Key("grow_record")]
    public GrowRecord GrowRecord { get; set; } = new();
}
