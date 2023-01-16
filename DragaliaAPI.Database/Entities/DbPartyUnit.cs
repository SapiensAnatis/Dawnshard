using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
public class DbPartyUnit
{
    // In theory, a composite primary key of [Party, UnitNo] would work great.
    // However, EF Core doesn't like navigation properties being used as keys.
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey($"{nameof(DeviceAccountId)},{nameof(PartyNo)}")]
    public virtual DbParty? Party { get; set; }

    public string DeviceAccountId { get; set; } = string.Empty;

    public int PartyNo { get; set; }

    public required int UnitNo { get; set; }

    public required Charas CharaId { get; set; }

    public long EquipDragonKeyId { get; set; } = 0;

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
