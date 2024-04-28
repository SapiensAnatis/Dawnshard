using MessagePack;

namespace DragaliaAPI.DTO;

public partial class CharaBuildupManaRequest
{
    [Key("mana_circle_piece_id_list")]
    public IList<int> ManaCirclePieceIdList { get; set; } = [];
}
