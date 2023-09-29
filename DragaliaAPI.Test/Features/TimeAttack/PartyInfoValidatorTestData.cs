using System.Text.Json;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Test.Features.TimeAttack;

public static class PartyInfoValidatorTestData
{
    // I can't be bothered to convert this to an object initializer. Sorry.
    public static readonly PartyInfo Valid = JsonSerializer.Deserialize<PartyInfo>(
        """
        {
           "party_unit_list": [
              {
                 "position": 1,
                 "chara_data": {
                     "chara_id": 10550101,
                     "exp": 8866950,
                     "level": 100,
                     "additional_max_level": 20,
                     "hp": 898,
                     "attack": 612,
                     "ex_ability_level": 5,
                     "ex_ability_2_level": 5,
                     "ability_1_level": 3,
                     "ability_2_level": 3,
                     "ability_3_level": 3,
                     "is_new": 1,
                     "skill_1_level": 4,
                     "skill_2_level": 3,
                     "burst_attack_level": 2,
                     "rarity": 5,
                     "limit_break_count": 5,
                     "hp_plus_count": 100,
                     "attack_plus_count": 100,
                     "status_plus_count": 0,
                     "combo_buildup_count": 1,
                     "is_unlock_edit_skill": 1,
                     "gettime": 1585659322,
                     "mana_circle_piece_id_list": [
                         1,
                         2,
                         3,
                         4,
                         5,
                         6,
                         7,
                         8,
                         9,
                         10,
                         11,
                         12,
                         13,
                         14,
                         15,
                         16,
                         17,
                         18,
                         19,
                         20,
                         21,
                         22,
                         23,
                         24,
                         25,
                         26,
                         27,
                         28,
                         29,
                         30,
                         31,
                         32,
                         33,
                         34,
                         35,
                         36,
                         37,
                         38,
                         39,
                         40,
                         41,
                         42,
                         43,
                         44,
                         45,
                         46,
                         47,
                         48,
                         49,
                         50,
                         51,
                         52,
                         53,
                         54,
                         55,
                         56,
                         57,
                         58,
                         59,
                         60,
                         61,
                         62,
                         63,
                         64,
                         65,
                         66,
                         67,
                         68,
                         69,
                         70
                     ],
                     "is_temporary": 0,
                     "list_view_flag": 1
                 },
                 "dragon_data": {
                     "dragon_id": 20050113,
                     "dragon_key_id": 4870,
                     "level": 100,
                     "exp": 1240020,
                     "is_lock": 1,
                     "is_new": 1,
                     "get_time": 1587964965,
                     "skill_1_level": 2,
                     "ability_1_level": 5,
                     "ability_2_level": 5,
                     "limit_break_count": 4,
                     "hp_plus_count": 50,
                     "attack_plus_count": 50,
                     "status_plus_count": 0
                 },
                 "weapon_skin_data": {
                     "weapon_skin_id": 0
                 },
                 "weapon_body_data": {
                     "weapon_body_id": 30560101,
                     "buildup_count": 90,
                     "limit_break_count": 9,
                     "limit_over_count": 2,
                     "skill_no": 1,
                     "skill_level": 2,
                     "ability_1_level": 3,
                     "ability_2_level": 3,
                     "equipable_count": 4,
                     "additional_crest_slot_type_1_count": 1,
                     "additional_crest_slot_type_2_count": 0,
                     "additional_crest_slot_type_3_count": 2,
                     "additional_effect_count": 0
                 },
                 "crest_slot_type_1_crest_list": [
                     {
                         "ability_crest_id": 40050125,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050101,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050073,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_2_crest_list": [
                     {
                         "ability_crest_id": 40040018,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40040042,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_3_crest_list": [
                     {
                         "ability_crest_id": 40090007,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     },
                     {
                         "ability_crest_id": 40090001,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     }
                 ],
                 "talisman_data": {
                     "talisman_key_id": 6742,
                     "talisman_id": 50540103,
                     "talisman_ability_id_1": 340000010,
                     "talisman_ability_id_2": 340000060,
                     "talisman_ability_id_3": 0,
                     "additional_hp": 0,
                     "additional_attack": 0,
                     "is_new": 1,
                     "is_lock": 0,
                     "gettime": 1693854942
                 },
                 "edit_skill_1_chara_data": {
                     "chara_id": 0,
                     "edit_skill_level": 0
                 },
                 "edit_skill_2_chara_data": {
                     "chara_id": 10540402,
                     "edit_skill_level": 2
                 },
                 "game_weapon_passive_ability_list": [
                     {
                         "weapon_passive_ability_id": 1050101
                     },
                     {
                         "weapon_passive_ability_id": 1050102
                     },
                     {
                         "weapon_passive_ability_id": 1050103
                     },
                     {
                         "weapon_passive_ability_id": 1050104
                     },
                     {
                         "weapon_passive_ability_id": 1050105
                     },
                     {
                         "weapon_passive_ability_id": 1050106
                     },
                     {
                         "weapon_passive_ability_id": 1050107
                     },
                     {
                         "weapon_passive_ability_id": 1050108
                     },
                     {
                         "weapon_passive_ability_id": 1050109
                     }
                 ],
                 "dragon_reliability_level": 30
             },
             {
                 "position": 2,
                 "chara_data": {
                     "chara_id": 10840102,
                     "exp": 1191950,
                     "level": 80,
                     "additional_max_level": 0,
                     "hp": 781,
                     "attack": 454,
                     "ex_ability_level": 5,
                     "ex_ability_2_level": 5,
                     "ability_1_level": 2,
                     "ability_2_level": 2,
                     "ability_3_level": 1,
                     "is_new": 1,
                     "skill_1_level": 3,
                     "skill_2_level": 2,
                     "burst_attack_level": 2,
                     "rarity": 5,
                     "limit_break_count": 4,
                     "hp_plus_count": 100,
                     "attack_plus_count": 100,
                     "status_plus_count": 0,
                     "combo_buildup_count": 0,
                     "is_unlock_edit_skill": 1,
                     "gettime": 1571399822,
                     "mana_circle_piece_id_list": [
                         1,
                         2,
                         3,
                         4,
                         5,
                         6,
                         7,
                         8,
                         9,
                         10,
                         11,
                         12,
                         13,
                         14,
                         15,
                         16,
                         17,
                         18,
                         19,
                         20,
                         21,
                         22,
                         23,
                         24,
                         25,
                         26,
                         27,
                         28,
                         29,
                         30,
                         31,
                         32,
                         33,
                         34,
                         35,
                         36,
                         37,
                         38,
                         39,
                         40,
                         41,
                         42,
                         43,
                         44,
                         45,
                         46,
                         47,
                         48,
                         49,
                         50
                     ],
                     "is_temporary": 0,
                     "list_view_flag": 1
                 },
                 "dragon_data": {
                     "dragon_id": 20050115,
                     "dragon_key_id": 4883,
                     "level": 100,
                     "exp": 1240020,
                     "is_lock": 1,
                     "is_new": 1,
                     "get_time": 1601274076,
                     "skill_1_level": 2,
                     "ability_1_level": 5,
                     "ability_2_level": 0,
                     "limit_break_count": 4,
                     "hp_plus_count": 50,
                     "attack_plus_count": 50,
                     "status_plus_count": 0
                 },
                 "weapon_skin_data": {
                     "weapon_skin_id": 0
                 },
                 "weapon_body_data": {
                     "weapon_body_id": 30860101,
                     "buildup_count": 90,
                     "limit_break_count": 9,
                     "limit_over_count": 2,
                     "skill_no": 1,
                     "skill_level": 2,
                     "ability_1_level": 3,
                     "ability_2_level": 3,
                     "equipable_count": 4,
                     "additional_crest_slot_type_1_count": 1,
                     "additional_crest_slot_type_2_count": 0,
                     "additional_crest_slot_type_3_count": 2,
                     "additional_effect_count": 0
                 },
                 "crest_slot_type_1_crest_list": [
                     {
                         "ability_crest_id": 40050008,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050080,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050020,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_2_crest_list": [
                     {
                         "ability_crest_id": 40040062,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40030004,
                         "buildup_count": 20,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_3_crest_list": [
                     {
                         "ability_crest_id": 40090018,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     },
                     {
                         "ability_crest_id": 40090022,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     }
                 ],
                 "talisman_data": {
                     "talisman_key_id": 6880,
                     "talisman_id": 50850104,
                     "talisman_ability_id_1": 340000070,
                     "talisman_ability_id_2": 340000134,
                     "talisman_ability_id_3": 0,
                     "additional_hp": 0,
                     "additional_attack": 0,
                     "is_new": 1,
                     "is_lock": 0,
                     "gettime": 1693854942
                 },
                 "edit_skill_1_chara_data": {
                     "chara_id": 10840501,
                     "edit_skill_level": 4
                 },
                 "edit_skill_2_chara_data": {
                     "chara_id": 10440301,
                     "edit_skill_level": 4
                 },
                 "game_weapon_passive_ability_list": [
                     {
                         "weapon_passive_ability_id": 1080101
                     },
                     {
                         "weapon_passive_ability_id": 1080102
                     },
                     {
                         "weapon_passive_ability_id": 1080103
                     },
                     {
                         "weapon_passive_ability_id": 1080104
                     },
                     {
                         "weapon_passive_ability_id": 1080105
                     },
                     {
                         "weapon_passive_ability_id": 1080106
                     },
                     {
                         "weapon_passive_ability_id": 1080107
                     },
                     {
                         "weapon_passive_ability_id": 1080108
                     },
                     {
                         "weapon_passive_ability_id": 1080109
                     }
                 ],
                 "dragon_reliability_level": 30
             },
             {
                 "position": 3,
                 "chara_data": {
                     "chara_id": 10950101,
                     "exp": 1191950,
                     "level": 80,
                     "additional_max_level": 0,
                     "hp": 754,
                     "attack": 505,
                     "ex_ability_level": 5,
                     "ex_ability_2_level": 5,
                     "ability_1_level": 2,
                     "ability_2_level": 2,
                     "ability_3_level": 2,
                     "is_new": 1,
                     "skill_1_level": 3,
                     "skill_2_level": 2,
                     "burst_attack_level": 2,
                     "rarity": 5,
                     "limit_break_count": 4,
                     "hp_plus_count": 100,
                     "attack_plus_count": 100,
                     "status_plus_count": 0,
                     "combo_buildup_count": 0,
                     "is_unlock_edit_skill": 1,
                     "gettime": 1609137524,
                     "mana_circle_piece_id_list": [
                         1,
                         2,
                         3,
                         4,
                         5,
                         6,
                         7,
                         8,
                         9,
                         10,
                         11,
                         12,
                         13,
                         14,
                         15,
                         16,
                         17,
                         18,
                         19,
                         20,
                         21,
                         22,
                         23,
                         24,
                         25,
                         26,
                         27,
                         28,
                         29,
                         30,
                         31,
                         32,
                         33,
                         34,
                         35,
                         36,
                         37,
                         38,
                         39,
                         40,
                         41,
                         42,
                         43,
                         44,
                         45,
                         46,
                         47,
                         48,
                         49,
                         50
                     ],
                     "is_temporary": 0,
                     "list_view_flag": 1
                 },
                 "dragon_data": {
                     "dragon_id": 20050113,
                     "dragon_key_id": 4877,
                     "level": 100,
                     "exp": 1240020,
                     "is_lock": 1,
                     "is_new": 1,
                     "get_time": 1594802928,
                     "skill_1_level": 2,
                     "ability_1_level": 5,
                     "ability_2_level": 5,
                     "limit_break_count": 4,
                     "hp_plus_count": 50,
                     "attack_plus_count": 50,
                     "status_plus_count": 0
                 },
                 "weapon_skin_data": {
                     "weapon_skin_id": 0
                 },
                 "weapon_body_data": {
                     "weapon_body_id": 30960101,
                     "buildup_count": 90,
                     "limit_break_count": 9,
                     "limit_over_count": 2,
                     "skill_no": 1,
                     "skill_level": 2,
                     "ability_1_level": 3,
                     "ability_2_level": 3,
                     "equipable_count": 4,
                     "additional_crest_slot_type_1_count": 1,
                     "additional_crest_slot_type_2_count": 0,
                     "additional_crest_slot_type_3_count": 2,
                     "additional_effect_count": 0
                 },
                 "crest_slot_type_1_crest_list": [
                     {
                         "ability_crest_id": 40050125,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050106,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050105,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_2_crest_list": [
                     {
                         "ability_crest_id": 40040003,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40040090,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_3_crest_list": [
                     {
                         "ability_crest_id": 40090007,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     },
                     {
                         "ability_crest_id": 40090001,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     }
                 ],
                 "talisman_data": {
                     "talisman_key_id": 0,
                     "talisman_id": 0,
                     "talisman_ability_id_1": 0,
                     "talisman_ability_id_2": 0,
                     "talisman_ability_id_3": 0,
                     "additional_hp": 0,
                     "additional_attack": 0,
                     "is_new": 0,
                     "is_lock": 0,
                     "gettime": -2147483648
                 },
                 "edit_skill_1_chara_data": {
                     "chara_id": 0,
                     "edit_skill_level": 0
                 },
                 "edit_skill_2_chara_data": {
                     "chara_id": 10540402,
                     "edit_skill_level": 2
                 },
                 "game_weapon_passive_ability_list": [
                     {
                         "weapon_passive_ability_id": 1090101
                     },
                     {
                         "weapon_passive_ability_id": 1090102
                     },
                     {
                         "weapon_passive_ability_id": 1090103
                     },
                     {
                         "weapon_passive_ability_id": 1090104
                     },
                     {
                         "weapon_passive_ability_id": 1090105
                     },
                     {
                         "weapon_passive_ability_id": 1090106
                     },
                     {
                         "weapon_passive_ability_id": 1090107
                     },
                     {
                         "weapon_passive_ability_id": 1090108
                     },
                     {
                         "weapon_passive_ability_id": 1090109
                     }
                 ],
                 "dragon_reliability_level": 30
             },
             {
                 "position": 4,
                 "chara_data": {
                     "chara_id": 10550103,
                     "exp": 1191950,
                     "level": 80,
                     "additional_max_level": 0,
                     "hp": 789,
                     "attack": 486,
                     "ex_ability_level": 5,
                     "ex_ability_2_level": 5,
                     "ability_1_level": 2,
                     "ability_2_level": 2,
                     "ability_3_level": 2,
                     "is_new": 1,
                     "skill_1_level": 3,
                     "skill_2_level": 2,
                     "burst_attack_level": 2,
                     "rarity": 5,
                     "limit_break_count": 4,
                     "hp_plus_count": 100,
                     "attack_plus_count": 100,
                     "status_plus_count": 0,
                     "combo_buildup_count": 0,
                     "is_unlock_edit_skill": 1,
                     "gettime": 1610653316,
                     "mana_circle_piece_id_list": [
                         1,
                         2,
                         3,
                         4,
                         5,
                         6,
                         7,
                         8,
                         9,
                         10,
                         11,
                         12,
                         13,
                         14,
                         15,
                         16,
                         17,
                         18,
                         19,
                         20,
                         21,
                         22,
                         23,
                         24,
                         25,
                         26,
                         27,
                         28,
                         29,
                         30,
                         31,
                         32,
                         33,
                         34,
                         35,
                         36,
                         37,
                         38,
                         39,
                         40,
                         41,
                         42,
                         43,
                         44,
                         45,
                         46,
                         47,
                         48,
                         49,
                         50
                     ],
                     "is_temporary": 0,
                     "list_view_flag": 1
                 },
                 "dragon_data": {
                     "dragon_id": 20050114,
                     "dragon_key_id": 4887,
                     "level": 100,
                     "exp": 1240020,
                     "is_lock": 1,
                     "is_new": 1,
                     "get_time": 1601887180,
                     "skill_1_level": 2,
                     "ability_1_level": 5,
                     "ability_2_level": 0,
                     "limit_break_count": 4,
                     "hp_plus_count": 50,
                     "attack_plus_count": 50,
                     "status_plus_count": 0
                 },
                 "weapon_skin_data": {
                     "weapon_skin_id": 0
                 },
                 "weapon_body_data": {
                     "weapon_body_id": 30560101,
                     "buildup_count": 90,
                     "limit_break_count": 9,
                     "limit_over_count": 2,
                     "skill_no": 1,
                     "skill_level": 2,
                     "ability_1_level": 3,
                     "ability_2_level": 3,
                     "equipable_count": 4,
                     "additional_crest_slot_type_1_count": 1,
                     "additional_crest_slot_type_2_count": 0,
                     "additional_crest_slot_type_3_count": 2,
                     "additional_effect_count": 0
                 },
                 "crest_slot_type_1_crest_list": [
                     {
                         "ability_crest_id": 40050058,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050125,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40050049,
                         "buildup_count": 50,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_2_crest_list": [
                     {
                         "ability_crest_id": 40040102,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     },
                     {
                         "ability_crest_id": 40040003,
                         "buildup_count": 40,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 50,
                         "attack_plus_count": 50
                     }
                 ],
                 "crest_slot_type_3_crest_list": [
                     {
                         "ability_crest_id": 40090007,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     },
                     {
                         "ability_crest_id": 40090002,
                         "buildup_count": 30,
                         "limit_break_count": 4,
                         "equipable_count": 4,
                         "ability_1_level": 3,
                         "ability_2_level": 3,
                         "hp_plus_count": 40,
                         "attack_plus_count": 40
                     }
                 ],
                 "talisman_data": {
                     "talisman_key_id": 0,
                     "talisman_id": 0,
                     "talisman_ability_id_1": 0,
                     "talisman_ability_id_2": 0,
                     "talisman_ability_id_3": 0,
                     "additional_hp": 0,
                     "additional_attack": 0,
                     "is_new": 0,
                     "is_lock": 0,
                     "gettime": -2147483648
                 },
                 "edit_skill_1_chara_data": {
                     "chara_id": 10850502,
                     "edit_skill_level": 4
                 },
                 "edit_skill_2_chara_data": {
                     "chara_id": 10840501,
                     "edit_skill_level": 4
                 },
                 "game_weapon_passive_ability_list": [
                     {
                         "weapon_passive_ability_id": 1050101
                     },
                     {
                         "weapon_passive_ability_id": 1050102
                     },
                     {
                         "weapon_passive_ability_id": 1050103
                     },
                     {
                         "weapon_passive_ability_id": 1050104
                     },
                     {
                         "weapon_passive_ability_id": 1050105
                     },
                     {
                         "weapon_passive_ability_id": 1050106
                     },
                     {
                         "weapon_passive_ability_id": 1050107
                     },
                     {
                         "weapon_passive_ability_id": 1050108
                     },
                     {
                         "weapon_passive_ability_id": 1050109
                     }
                 ],
                 "dragon_reliability_level": 30
             }
         ]
     }
     """,
        ApiJsonOptions.Instance
    )!;
}
