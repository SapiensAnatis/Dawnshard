using MessagePack;

namespace DragaliaAPI.DTO;

public partial class EventTradeGetListResponse
{
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}
