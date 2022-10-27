using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Models.Data.Entity;

namespace DragaliaAPI.Models.Database.Savefile;

public class DbPartyUnit
{
    // In theory, a composite primary key of [Party, UnitNo] would work great.
    // However, EF Core doesn't like navigation properties being used as keys.
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public virtual DbParty Party { get; set; } = null!;

    [Required]
    public int UnitNo { get; set; }

    [Required]
    public Charas CharaId { get; set; }

    public long EquipDragonKeyId { get; set; }

    public int EquipWeaponBodyId { get; set; }

    public int EquipWeaponSkinId { get; set; }

    public int EquipCrestSlotType1CrestId1 { get; set; }

    public int EquipCrestSlotType1CrestId2 { get; set; }

    public int EquipCrestSlotType1CrestId3 { get; set; }

    public int EquipCrestSlotType2CrestId1 { get; set; }

    public int EquipCrestSlotType2CrestId2 { get; set; }

    public int EquipCrestSlotType3CrestId1 { get; set; }

    public int EquipCrestSlotType3CrestId2 { get; set; }

    public long EquipTalismanKeyId { get; set; }

    public int EditSkill1CharaId { get; set; }

    public int EditSkill2CharaId { get; set; }
}
