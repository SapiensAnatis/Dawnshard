using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(QuestId), nameof(IsMulti), nameof(UnitNo))]
public class DbQuestClearPartyUnit : DbPartyUnitBase, IDbPlayerData
{
    public virtual DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(Owner))]
    public required long ViewerId { get; set; }

    public required int QuestId { get; set; }

    public required bool IsMulti { get; set; }

    public Dragons EquippedDragonEntityId { get; set; }

    public Talismans EquippedTalismanEntityId { get; set; }

    /// <summary>
    /// Reset this entity back to a state representing an empty slot.
    /// </summary>
    public void Clear()
    {
        this.CharaId = Charas.Empty;

        this.EquipWeaponBodyId = WeaponBodies.Empty;
        this.EquipWeaponSkinId = 0;

        this.EquipDragonKeyId = 0;
        this.EquipTalismanKeyId = 0;
        this.EquippedDragonEntityId = Dragons.Empty;
        this.EquippedTalismanEntityId = Talismans.Empty;

        this.EquipCrestSlotType1CrestId1 = AbilityCrests.Empty;
        this.EquipCrestSlotType1CrestId2 = AbilityCrests.Empty;
        this.EquipCrestSlotType1CrestId3 = AbilityCrests.Empty;

        this.EquipCrestSlotType2CrestId1 = AbilityCrests.Empty;
        this.EquipCrestSlotType2CrestId2 = AbilityCrests.Empty;

        this.EquipCrestSlotType3CrestId1 = AbilityCrests.Empty;
        this.EquipCrestSlotType3CrestId2 = AbilityCrests.Empty;

        this.EditSkill1CharaId = Charas.Empty;
        this.EditSkill2CharaId = Charas.Empty;
    }
}
