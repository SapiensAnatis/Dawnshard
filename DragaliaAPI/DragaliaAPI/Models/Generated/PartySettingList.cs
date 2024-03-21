using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class PartySettingList
{
    [Obsolete]
    [Key("equip_weapon_key_id")]
    public ulong EquipWeaponKeyId { get; set; }

    [Obsolete]
    [Key("equip_skin_weapon_id")]
    public int EquipSkinWeaponId { get; set; }

    [Obsolete]
    [Key("equip_amulet_key_id")]
    public ulong EquipAmuletKeyId { get; set; }

    [Obsolete]
    [Key("equip_amulet_2_key_id")]
    public ulong EquipAmulet2KeyId { get; set; }

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
