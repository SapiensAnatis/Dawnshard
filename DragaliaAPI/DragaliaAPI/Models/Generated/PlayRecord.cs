using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class PlayRecord
{
    [Key("live_unit_no_list")]
    public IReadOnlyList<int> LiveUnitNoList { get; set; } = [];
}
