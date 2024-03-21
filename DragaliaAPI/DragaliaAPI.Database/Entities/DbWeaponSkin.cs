using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(WeaponSkinId))]
public class DbWeaponSkin : DbPlayerData
{
    public int WeaponSkinId { get; set; }

    public bool IsNew { get; set; }

    public DateTimeOffset GetTime { get; set; }
}
