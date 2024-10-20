using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class AtgenDropAll
{
    /// <summary>
    /// Gets or sets a value that causes an additional pop-up against the drop
    /// </summary>
    /// <remarks>
    /// Known values:
    /// <ul>
    /// <li>0: Normal</li>
    /// <li>1: Capacity Reached + Rainbow effect (???)</li>
    /// <li>9: Bonus</li>
    /// </ul>
    /// Might correspond to RewardItemPlaceType in il2cpp.
    /// </remarks>
    [Key("place")]
    public int Place { get; set; }

    [Key("factor")]
    public float Factor { get; set; }
}
