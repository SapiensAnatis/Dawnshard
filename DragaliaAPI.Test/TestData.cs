using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test;

public static class TestData
{
    public static UserSupportList supportListEuden =
        new()
        {
            viewer_id = 1000,
            name = "Euden",
            level = 10,
            last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            emblem_id = 40000002,
            max_party_power = 9999,
            support_chara = new()
            {
                chara_id = Charas.ThePrince,
                level = 10,
                additional_max_level = 0,
                rarity = 5,
                hp = 60,
                attack = 40,
                hp_plus_count = 0,
                attack_plus_count = 0,
                ability_1_level = 0,
                ability_2_level = 0,
                ability_3_level = 0,
                ex_ability_level = 1,
                ex_ability_2_level = 1,
                skill_1_level = 1,
                skill_2_level = 0,
                is_unlock_edit_skill = true
            },
            support_dragon = new() { dragon_id = Dragons.Midgardsormr },
            support_weapon_body = new() { weapon_body_id = WeaponBodies.SoldiersBrand },
            support_talisman = new() { talisman_id = Talismans.ThePrince },
            support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { ability_crest_id = AbilityCrests.TheGreatestGift },
                new() { ability_crest_id = 0 },
                new() { ability_crest_id = 0 },
            },
            support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { ability_crest_id = AbilityCrests.ManaFount },
                new() { ability_crest_id = 0 },
            },
            support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { ability_crest_id = 0 },
                new() { ability_crest_id = 0 },
            },
            guild = new() { guild_id = 0, guild_name = "Guild" }
        };

    public static UserSupportList supportListElisanne =
        new()
        {
            viewer_id = 1001,
            name = "Elisanne",
            level = 10,
            last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            emblem_id = 40000002,
            max_party_power = 9999,
            support_chara = new()
            {
                chara_id = Charas.Elisanne,
                level = 10,
                additional_max_level = 0,
                rarity = 5,
                hp = 60,
                attack = 40,
                hp_plus_count = 0,
                attack_plus_count = 0,
                ability_1_level = 0,
                ability_2_level = 0,
                ability_3_level = 0,
                ex_ability_level = 1,
                ex_ability_2_level = 1,
                skill_1_level = 1,
                skill_2_level = 0,
                is_unlock_edit_skill = true
            },
            support_dragon = new() { dragon_key_id = 0, },
            support_weapon_body = new() { weapon_body_id = 0, },
            support_talisman = new() { talisman_key_id = 0, },
            support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { ability_crest_id = 0 },
                new() { ability_crest_id = 0 },
                new() { ability_crest_id = 0 },
            },
            support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { ability_crest_id = 0 },
                new() { ability_crest_id = 0 },
            },
            support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { ability_crest_id = 0 },
                new() { ability_crest_id = 0 },
            },
            guild = new() { guild_id = 0, guild_name = "Guild" }
        };
}
