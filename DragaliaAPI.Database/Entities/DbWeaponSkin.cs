using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId), nameof(WeaponSkinId))]
[Index(nameof(DeviceAccountId))]
public class DbWeaponSkin : IDbHasAccountId
{
    public virtual DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    public int WeaponSkinId { get; set; }

    public bool IsNew { get; set; }

    public DateTimeOffset GetTime { get; set; }
}
