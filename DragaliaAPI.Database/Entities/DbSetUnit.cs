using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

//TODO: This and PartyUnit share a lot of properties, maybe extract those and make these into subclasses which inherit them
[Table("SetUnit")]
public class DbSetUnit : IDbHasAccountId
{
    [Required]
    [ForeignKey("DbDeviceAccountId")]
    public string DeviceAccountId { get; set; } = null!;

    [Required]
    public Charas CharaId { get; set; }

    [Required]
    public int UnitSetNo { get; set; }

    public string UnitSetName { get; set; } = null!;

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
}
