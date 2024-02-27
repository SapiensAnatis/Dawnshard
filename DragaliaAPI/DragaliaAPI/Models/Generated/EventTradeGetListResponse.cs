using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class EventTradeGetListResponse
{
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}
