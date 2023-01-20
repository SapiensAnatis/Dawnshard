using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
public class DbTalisman : IDbHasAccountId
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public long TalismanKeyId { get; set; }

    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    public required Talismans TalismanId { get; set; }

    public int TalismanAbilityId1 { get; set; }

    public int TalismanAbilityId2 { get; set; }

    public int TalismanAbilityId3 { get; set; }

    public int AdditionalHp { get; set; } = 0;

    public int AdditionalAttack { get; set; } = 0;

    public bool IsNew { get; set; } = true;

    public bool IsLock { get; set; } = false;

    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UtcNow;
}
