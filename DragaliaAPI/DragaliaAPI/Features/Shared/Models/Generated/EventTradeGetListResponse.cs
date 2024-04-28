using DragaliaAPI.Features.Shared.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Features.Shared.Models.Generated;

public partial class EventTradeGetListResponse
{
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}
