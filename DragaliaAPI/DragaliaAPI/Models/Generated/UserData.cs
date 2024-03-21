using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class UserData
{
    [Key("max_weapon_quantity")]
    [Obsolete]
    public int MaxWeaponQuantity { get; set; }

    [Key("max_amulet_quantity")]
    [Obsolete]
    public int MaxAmuletQuantity { get; set; }
}
