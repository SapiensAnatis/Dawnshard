using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class EventTradeTradeResponse
{
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}
