using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

public class DbTalisman : DbPlayerData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long TalismanKeyId { get; set; }

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
