using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

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

    public WeaponBodies EquipWeaponBodyId { get; set; }

    public int EquipWeaponSkinId { get; set; }

    public AbilityCrests EquipCrestSlotType1CrestId1 { get; set; }

    public AbilityCrests EquipCrestSlotType1CrestId2 { get; set; }

    public AbilityCrests EquipCrestSlotType1CrestId3 { get; set; }

    public AbilityCrests EquipCrestSlotType2CrestId1 { get; set; }

    public AbilityCrests EquipCrestSlotType2CrestId2 { get; set; }

    public AbilityCrests EquipCrestSlotType3CrestId1 { get; set; }

    public AbilityCrests EquipCrestSlotType3CrestId2 { get; set; }

    public long EquipTalismanKeyId { get; set; }

    public Charas EditSkill1CharaId { get; set; }

    public Charas EditSkill2CharaId { get; set; }
}
