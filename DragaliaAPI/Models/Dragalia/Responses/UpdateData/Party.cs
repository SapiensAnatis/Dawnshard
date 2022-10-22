using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Database.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record Party(int party_no, string party_name, IEnumerable<PartyUnit> party_setting_list);

[MessagePackObject(true)]
public record PartyUnit(
    int unit_no,
    int chara_id,
    ulong equip_dragon_key_id,
    int equip_weapon_body_id,
    int equip_weapon_skin_id,
    int equip_crest_slot_type_1_crest_id_1,
    int equip_crest_slot_type_1_crest_id_2,
    int equip_crest_slot_type_1_crest_id_3,
    int equip_crest_slot_type_2_crest_id_1,
    int equip_crest_slot_type_2_crest_id_2,
    int equip_crest_slot_type_3_crest_id_1,
    int equip_crest_slot_type_3_crest_id_2,
    ulong equip_talisman_key_id,
    int edit_skill_1_chara_id,
    int edit_skill_2_chara_id
);

public static class PartyFactory
{
    public static Party CreateDto(DbParty dbEntry)
    {
        List<PartyUnit> UnitList = dbEntry.Units.Select(CreateDtoUnit).ToList();

        // Fill empty slots
        for (int i = UnitList.Count + 1; i <= 4; i++)
        {
            UnitList.Add(Empty with { unit_no = i });
        }

        return new Party(dbEntry.PartyNo, dbEntry.PartyName, UnitList);
    }

    public static DbParty CreateDbEntry(string deviceAccountId, Party party)
    {
        return new DbParty()
        {
            DeviceAccountId = deviceAccountId,
            PartyName = party.party_name,
            PartyNo = party.party_no,
            Units = party.party_setting_list.Select(CreateDbUnit).ToList()
        };
    }

    private static PartyUnit Empty => new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

    private static PartyUnit CreateDtoUnit(DbPartyUnit dbEntry)
    {
        return new PartyUnit(
            unit_no: dbEntry.UnitNo,
            chara_id: (int)dbEntry.CharaId,
            equip_dragon_key_id: (ulong)dbEntry.EquipDragonKeyId,
            equip_weapon_body_id: dbEntry.EquipWeaponBodyId,
            equip_weapon_skin_id: dbEntry.EquipWeaponSkinId,
            equip_crest_slot_type_1_crest_id_1: dbEntry.EquipCrestSlotType1CrestId1,
            equip_crest_slot_type_1_crest_id_2: dbEntry.EquipCrestSlotType1CrestId2,
            equip_crest_slot_type_1_crest_id_3: dbEntry.EquipCrestSlotType1CrestId3,
            equip_crest_slot_type_2_crest_id_1: dbEntry.EquipCrestSlotType2CrestId1,
            equip_crest_slot_type_2_crest_id_2: dbEntry.EquipCrestSlotType2CrestId2,
            equip_crest_slot_type_3_crest_id_1: dbEntry.EquipCrestSlotType3CrestId1,
            equip_crest_slot_type_3_crest_id_2: dbEntry.EquipCrestSlotType3CrestId2,
            equip_talisman_key_id: (ulong)dbEntry.EquipTalismanKeyId,
            edit_skill_1_chara_id: dbEntry.EditSkill1CharaId,
            edit_skill_2_chara_id: dbEntry.EditSkill2CharaId
        );
    }

    private static DbPartyUnit CreateDbUnit(PartyUnit unit)
    {
        return new DbPartyUnit()
        {
            UnitNo = unit.unit_no,
            CharaId = (Charas)unit.chara_id,
            EquipDragonKeyId = (long)unit.equip_dragon_key_id,
            EquipWeaponBodyId = unit.equip_weapon_body_id,
            EquipWeaponSkinId = unit.equip_weapon_skin_id,
            EquipCrestSlotType1CrestId1 = unit.equip_crest_slot_type_1_crest_id_1,
            EquipCrestSlotType1CrestId2 = unit.equip_crest_slot_type_1_crest_id_2,
            EquipCrestSlotType1CrestId3 = unit.equip_crest_slot_type_1_crest_id_3,
            EquipCrestSlotType2CrestId1 = unit.equip_crest_slot_type_2_crest_id_1,
            EquipCrestSlotType2CrestId2 = unit.equip_crest_slot_type_2_crest_id_2,
            EquipCrestSlotType3CrestId1 = unit.equip_crest_slot_type_3_crest_id_1,
            EquipCrestSlotType3CrestId2 = unit.equip_crest_slot_type_2_crest_id_2,
            EquipTalismanKeyId = (long)unit.equip_talisman_key_id,
            EditSkill1CharaId = unit.edit_skill_1_chara_id,
            EditSkill2CharaId = unit.edit_skill_2_chara_id,
        };
    }
}
