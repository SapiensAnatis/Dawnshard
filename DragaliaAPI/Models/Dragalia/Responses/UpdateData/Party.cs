using DragaliaAPI.Models.Database.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record Party(int party_no, string party_name, IEnumerable<PartyUnit> party_setting_list);

[MessagePackObject(true)]
public record PartyUnit(
    int unit_no,
    int chara_id,
    int equip_dragon_key_id,
    int equip_weapon_body_id,
    int equip_weapon_skin_id,
    int equip_crest_slot_type_1_crest_id_1,
    int equip_crest_slot_type_1_crest_id_2,
    int equip_crest_slot_type_1_crest_id_3,
    int equip_crest_slot_type_2_crest_id_1,
    int equip_crest_slot_type_2_crest_id_2,
    int equip_crest_slot_type_3_crest_id_1,
    int equip_crest_slot_type_3_crest_id_2,
    int equip_talisman_key_id,
    int edit_skill_1_chara_id,
    int edit_skill_2_chara_id
);

public static class PartyFactory
{
    public static Party Create(DbParty dbEntry)
    {
        List<PartyUnit> UnitList = dbEntry.Units.Select(CreateUnit).ToList();

        // Fill empty slots
        for (int i = UnitList.Count() + 1; i <= 4; i++)
        {
            UnitList.Add(Empty with { unit_no = i });
        }

        return new Party(dbEntry.PartyNo, dbEntry.PartyName, UnitList);
    }

    private static PartyUnit Empty => new PartyUnit(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

    private static PartyUnit CreateUnit(DbPartyUnit dbEntry)
    {
        return new PartyUnit(
            unit_no: dbEntry.UnitNo,
            chara_id: (int)dbEntry.CharaId,
            equip_dragon_key_id: dbEntry.EquipDragonKeyId,
            equip_weapon_body_id: dbEntry.EquipWeaponBodyId,
            equip_weapon_skin_id: dbEntry.EquipWeaponSkinId,
            equip_crest_slot_type_1_crest_id_1: dbEntry.EquipCrestSlotType1CrestId1,
            equip_crest_slot_type_1_crest_id_2: dbEntry.EquipCrestSlotType1CrestId2,
            equip_crest_slot_type_1_crest_id_3: dbEntry.EquipCrestSlotType1CrestId3,
            equip_crest_slot_type_2_crest_id_1: dbEntry.EquipCrestSlotType2CrestId1,
            equip_crest_slot_type_2_crest_id_2: dbEntry.EquipCrestSlotType2CrestId2,
            equip_crest_slot_type_3_crest_id_1: dbEntry.EquipCrestSlotType3CrestId1,
            equip_crest_slot_type_3_crest_id_2: dbEntry.EquipCrestSlotType3CrestId2,
            equip_talisman_key_id: dbEntry.EquipTalismanKeyId,
            edit_skill_1_chara_id: dbEntry.EditSkill1CharaId,
            edit_skill_2_chara_id: dbEntry.EditSkill2CharaId
        );
    }
}
