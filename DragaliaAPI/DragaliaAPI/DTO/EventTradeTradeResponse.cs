using MessagePack;

namespace DragaliaAPI.DTO;

public partial class EventTradeTradeResponse
{
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}
