using DragaliaAPI.Features.Shared.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Features.Shared.Models.Generated;

public partial class IngameResultData
{
    [Key("reward_record")]
    public RewardRecord RewardRecord { get; set; } = new();

    [Key("grow_record")]
    public GrowRecord GrowRecord { get; set; } = new();
}
