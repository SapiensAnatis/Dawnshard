using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities.Abstract;

/// <summary>
/// Base class for party units and unit equipment set entries.
/// </summary>
public abstract class DbUnitBase
{
    public Charas CharaId { get; set; }

    public long EquipDragonKeyId { get; set; }

    public WeaponBodies EquipWeaponBodyId { get; set; }

    public AbilityCrests EquipCrestSlotType1CrestId1 { get; set; }

    public AbilityCrests EquipCrestSlotType1CrestId2 { get; set; }

    public AbilityCrests EquipCrestSlotType1CrestId3 { get; set; }

    public AbilityCrests EquipCrestSlotType2CrestId1 { get; set; }

    public AbilityCrests EquipCrestSlotType2CrestId2 { get; set; }

    public AbilityCrests EquipCrestSlotType3CrestId1 { get; set; }

    public AbilityCrests EquipCrestSlotType3CrestId2 { get; set; }

    public long EquipTalismanKeyId { get; set; }
}
