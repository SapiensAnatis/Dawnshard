using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Database.Utils;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using System;

namespace DragaliaAPI.Services;

public class HelperService : IHelperService
{
    private readonly IMapper mapper;

    public HelperService(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public async Task<QuestGetSupportUserListData> GetHelpers()
    {
        // TODO: Make this actually pull from database
        await Task.CompletedTask;

        return StubData.SupportListData;
    }

    public AtgenSupportData BuildHelperData(
        UserSupportList helperInfo,
        AtgenSupportUserDetailList helperDetails
    )
    {
        return new AtgenSupportData()
        {
            viewer_id = helperInfo.viewer_id,
            name = helperInfo.name,
            is_friend = helperDetails.is_friend,
            chara_data = this.mapper.Map<CharaList>(helperInfo.support_chara),
            dragon_data = this.mapper.Map<DragonList>(helperInfo.support_dragon),
            weapon_body_data = this.mapper.Map<GameWeaponBody>(helperInfo.support_weapon_body),
            crest_slot_type_1_crest_list = helperInfo.support_crest_slot_type_1_list.Select(
                this.mapper.Map<GameAbilityCrest>
            ),
            crest_slot_type_2_crest_list = helperInfo.support_crest_slot_type_2_list.Select(
                this.mapper.Map<GameAbilityCrest>
            ),
            crest_slot_type_3_crest_list = helperInfo.support_crest_slot_type_3_list.Select(
                this.mapper.Map<GameAbilityCrest>
            ),
            talisman_data = this.mapper.Map<TalismanList>(helperInfo.support_talisman)
        };
    }

    private static class StubData
    {
        public static readonly QuestGetSupportUserListData SupportListData =
            new()
            {
                support_user_list = new List<UserSupportList>()
                {
                    new()
                    {
                        viewer_id = 1000,
                        name = "dreadfullydistinct",
                        level = 400,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 40000002,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.DragonyuleIlia,
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
                    },
                    new()
                    {
                        viewer_id = 1001,
                        name = "Nightmerp",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10250305,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.GalaEmile,
                            level = 80,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 789,
                            attack = 486,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 2,
                            ability_2_level = 2,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 3,
                            skill_2_level = 2,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.AqueousPrison,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.GalaEmile,
                            additional_attack = 100,
                            additional_hp = 100
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ARoyalTeaParty,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.QueenoftheBlueSeas,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PeacefulWaterfront,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.HisCleverBrother,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.GreatwyrmMercury,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1002,
                        name = "Alicia",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10150503,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Alberius,
                            level = 80,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 752,
                            attack = 506,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 2,
                            ability_2_level = 2,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 3,
                            skill_2_level = 2,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.Ramiel,
                            level = 120,
                            hp = 388,
                            attack = 148,
                            skill_1_level = 2,
                            ability_1_level = 6,
                            ability_2_level = 6,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 5
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.PrimalHex,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Alberius,
                            talisman_ability_id_1 = 340000030,
                            talisman_ability_id_2 = 340000132
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.WelcometotheOpera,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.AManUnchanging,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PrayersUntoHim,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ChariotDrift,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.DragonsNest,
                                buildup_count = 20,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1003,
                        name = "alkaemist",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10850503,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Grace,
                            level = 80,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 804,
                            attack = 470,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 2,
                            ability_2_level = 2,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 3,
                            skill_2_level = 2,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.Ramiel,
                            level = 120,
                            hp = 388,
                            attack = 148,
                            skill_1_level = 2,
                            ability_1_level = 6,
                            ability_2_level = 6,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 5
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.ConsumingDarkness,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Grace,
                            talisman_ability_id_1 = 340000070,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.GentleWinds,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.TheChocolatiers,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.AWidowsLament,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.DragonsNest,
                                buildup_count = 20,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1004,
                        name = "QwerbyKing",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10850502,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.SummerVerica,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 964,
                            attack = 563,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.Ramiel,
                            level = 120,
                            hp = 388,
                            attack = 148,
                            skill_1_level = 2,
                            ability_1_level = 6,
                            ability_2_level = 6,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 5
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.ConsumingDarkness,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.SummerVerica,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.DragonsNest,
                                buildup_count = 20,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1005,
                        name = "Zappypants",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10550103,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.KimonoElisanne,
                            level = 80,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 789,
                            attack = 486,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 2,
                            ability_2_level = 2,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 3,
                            skill_2_level = 2,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.PrimalBrunhilda,
                            level = 100,
                            hp = 369,
                            attack = 127,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.OmniflameLance,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.KimonoElisanne,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.SaintStarfallsCircus,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1006,
                        name = "stairs",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10550103,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Elisanne,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 902,
                            attack = 551,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.LimpidCascade,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Elisanne,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.SaintStarfallsCircus,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1007,
                        name = "jang",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10250302,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Tobias,
                            level = 80,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 781,
                            attack = 494,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 2,
                            ability_2_level = 2,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 3,
                            skill_2_level = 2,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.NobleHorizon,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Tobias,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.AChoiceBlend,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1008,
                        name = "Euden",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10850301,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Akasha,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 967,
                            attack = 561,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.NobleHorizon,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Akasha,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WhataHandful,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1009,
                        name = "Euden",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10850301,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Patia,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 961,
                            attack = 537,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.Ramiel,
                            level = 120,
                            hp = 388,
                            attack = 148,
                            skill_1_level = 2,
                            ability_1_level = 6,
                            ability_2_level = 6,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 5
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.EbonPlagueLance,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Patia,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.DragonsNest,
                                buildup_count = 20,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1010,
                        name = "Leon",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10850301,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Delphi,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 956,
                            attack = 572,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.Ramiel,
                            level = 120,
                            hp = 388,
                            attack = 148,
                            skill_1_level = 2,
                            ability_1_level = 6,
                            ability_2_level = 6,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 5
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.ShaderulersFang,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Delphi,
                            talisman_ability_id_1 = 340000030,
                            talisman_ability_id_2 = 340000132
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.WelcometotheOpera,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.AManUnchanging,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WorthyRivals,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ChariotDrift,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.DragonsNest,
                                buildup_count = 20,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1011,
                        name = "Crown",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10750201,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Lily,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 899,
                            attack = 613,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.AqueousPrison,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Lily,
                            talisman_ability_id_1 = 340000030,
                            talisman_ability_id_2 = 340000132
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.WingsofRebellionatRest,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WorthyRivals,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.SeasidePrincess,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ChariotDrift,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.LordoftheSkies,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1012,
                        name = "Euden",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10150303,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.GalaLeif,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 1002,
                            attack = 546,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.PrimalTempest,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.GalaLeif,
                            talisman_ability_id_1 = 340000030,
                            talisman_ability_id_2 = 340000132
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.AManUnchanging,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.GoingUndercover,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WorthyRivals,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ChariotDrift,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.LordoftheSkies,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1013,
                        name = "sockperson",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10350502,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Delphi,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 956,
                            attack = 572,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.Ramiel,
                            level = 120,
                            hp = 388,
                            attack = 148,
                            skill_1_level = 2,
                            ability_1_level = 6,
                            ability_2_level = 6,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 5
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.ShaderulersFang,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Delphi,
                            talisman_ability_id_1 = 340000030,
                            talisman_ability_id_2 = 340000132
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.WelcometotheOpera,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.AManUnchanging,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WorthyRivals,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ChariotDrift,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.DragonsNest,
                                buildup_count = 20,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1014,
                        name = "Delpolo",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10840402,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.Vixel,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 943,
                            attack = 542,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaElysium,
                            level = 100,
                            hp = 371,
                            attack = 124,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.CosmicRuler,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.Vixel,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WhataHandful,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1015,
                        name = "Euden",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10850402,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.GalaZena,
                            level = 100,
                            additional_max_level = 20,
                            rarity = 5,
                            hp = 553,
                            attack = 350,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 3,
                            ability_2_level = 3,
                            ability_3_level = 3,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 4,
                            skill_2_level = 3,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaElysium,
                            level = 100,
                            hp = 371,
                            attack = 124,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.CosmicRuler,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.GalaZena,
                            talisman_ability_id_1 = 340000010,
                            talisman_ability_id_2 = 340000134
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.CastleCheerCorps,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.StudyRabbits,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.ProperMaintenance,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.FromWhenceHeComes,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WhataHandful,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.RavenousFireCrownsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.PromisedPietyStaffsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    },
                    new()
                    {
                        viewer_id = 1016,
                        name = "Nahxela",
                        level = 250,
                        last_login_date = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
                        emblem_id = 10350303,
                        max_party_power = 9999,
                        support_chara = new()
                        {
                            chara_id = Charas.GalaNotte,
                            level = 80,
                            additional_max_level = 0,
                            rarity = 5,
                            hp = 760,
                            attack = 499,
                            hp_plus_count = 100,
                            attack_plus_count = 100,
                            ability_1_level = 2,
                            ability_2_level = 2,
                            ability_3_level = 2,
                            ex_ability_level = 5,
                            ex_ability_2_level = 5,
                            skill_1_level = 3,
                            skill_2_level = 2,
                            is_unlock_edit_skill = true
                        },
                        support_dragon = new()
                        {
                            dragon_key_id = 0,
                            dragon_id = Dragons.GalaBahamut,
                            level = 100,
                            hp = 368,
                            attack = 128,
                            skill_1_level = 2,
                            ability_1_level = 5,
                            ability_2_level = 5,
                            hp_plus_count = 50,
                            attack_plus_count = 50,
                            limit_break_count = 4
                        },
                        support_weapon_body = new()
                        {
                            weapon_body_id = WeaponBodies.WindrulersFang,
                            buildup_count = 80,
                            limit_break_count = 8,
                            limit_over_count = 1,
                            equipable_count = 4,
                            additional_crest_slot_type_1_count = 1,
                            additional_crest_slot_type_2_count = 0,
                            additional_crest_slot_type_3_count = 2
                        },
                        support_talisman = new()
                        {
                            talisman_key_id = 0,
                            talisman_id = Talismans.GalaNotte,
                            talisman_ability_id_1 = 340000030,
                            talisman_ability_id_2 = 340000132
                        },
                        support_crest_slot_type_1_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.GoingUndercover,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.AManUnchanging,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.WorthyRivals,
                                buildup_count = 50,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_2_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.ChariotDrift,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.LordoftheSkies,
                                buildup_count = 40,
                                limit_break_count = 4,
                                hp_plus_count = 50,
                                attack_plus_count = 50,
                                equipable_count = 4
                            },
                        },
                        support_crest_slot_type_3_list = new List<AtgenSupportCrestSlotType1List>()
                        {
                            new()
                            {
                                ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            },
                            new()
                            {
                                ability_crest_id = AbilityCrests.CrownofLightSerpentsBoon,
                                buildup_count = 30,
                                limit_break_count = 4,
                                hp_plus_count = 40,
                                attack_plus_count = 40,
                                equipable_count = 4
                            }
                        },
                        guild = new() { guild_id = 0, guild_name = "Guild" }
                    }
                },
                support_user_detail_list = new List<AtgenSupportUserDetailList>()
                {
                    new()
                    {
                        viewer_id = 1000,
                        gettable_mana_point = 25,
                        is_friend = false
                    },
                    new()
                    {
                        viewer_id = 1001,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1002,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1003,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1004,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1005,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1006,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1007,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1008,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1009,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1010,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1011,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1012,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1013,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1014,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1015,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                    new()
                    {
                        viewer_id = 1016,
                        gettable_mana_point = 25,
                        is_friend = true
                    },
                }
            };
    }
}
