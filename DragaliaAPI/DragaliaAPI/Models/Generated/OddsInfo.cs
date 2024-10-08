using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class OddsInfo
{
    [Key("enemy")]
    public IList<AtgenEnemy> Enemy { get; set; } = [];
}
