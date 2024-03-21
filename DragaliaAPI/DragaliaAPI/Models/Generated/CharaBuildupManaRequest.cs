using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class CharaBuildupManaRequest
{
    [Key("mana_circle_piece_id_list")]
    public IList<int> ManaCirclePieceIdList { get; set; } = [];
}
