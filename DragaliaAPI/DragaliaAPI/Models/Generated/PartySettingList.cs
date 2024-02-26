using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Models.Generated;

public partial class PartySettingList
{
    public IEnumerable<AbilityCrests> GetAbilityCrestList() =>
        new List<AbilityCrests>()
        {
            this.EquipCrestSlotType1CrestId1,
            this.EquipCrestSlotType1CrestId2,
            this.EquipCrestSlotType1CrestId3,
            this.EquipCrestSlotType2CrestId1,
            this.EquipCrestSlotType2CrestId2,
            this.EquipCrestSlotType3CrestId1,
            this.EquipCrestSlotType3CrestId2
        };
}
