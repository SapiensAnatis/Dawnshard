using MessagePack;

namespace DragaliaAPI.DTO;

public partial class CharaLimitBreakAndBuildupManaRequest
{
    [Key("mana_circle_piece_id_list")]
    public IList<int> ManaCirclePieceIdList { get; set; } = [];
}
