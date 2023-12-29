using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Quest;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="QuestController"/>
/// </summary>
public class QuestClearPartyTest : TestFixture
{
    public QuestClearPartyTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task GetQuestClearParty_ReturnsSetClearParty()
    {
        await this.ImportSave();

        await this.AddRangeToDatabase(SoloDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyData> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyData>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { quest_id = 1 }
            );

        response.data.quest_clear_party_setting_list.Should().BeEquivalentTo(SoloPartySettingLists);
        response.data.lost_unit_list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearPartyMulti_ReturnsSetClearParty()
    {
        await this.ImportSave();

        await this.AddRangeToDatabase(MultiDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyMultiData> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyMultiData>(
                "/quest/get_quest_clear_party_multi",
                new QuestGetQuestClearPartyRequest() { quest_id = 2 }
            );

        response
            .data.quest_multi_clear_party_setting_list.Should()
            .BeEquivalentTo(MultiPartySettingLists);
        response.data.lost_unit_list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearParty_DoesNotRetrieveMultiData()
    {
        int questId = 5;

        await this.Client.PostMsgpack<QuestSetQuestClearPartyData>(
            "/quest/set_quest_clear_party_multi",
            new QuestSetQuestClearPartyRequest()
            {
                quest_id = questId,
                request_party_setting_list = MultiPartySettingLists
            }
        );

        DragaliaResponse<QuestGetQuestClearPartyData> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyData>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { quest_id = questId }
            );

        response.data.quest_clear_party_setting_list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearParty_HandlesMissingEntities()
    {
        await this.ImportSave();

        int questId = MissingItemDbEntities[0].QuestId;

        await this.AddRangeToDatabase(MissingItemDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyData> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyData>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { quest_id = questId }
            );

        response
            .data.lost_unit_list.Should()
            .BeEquivalentTo(
                new List<AtgenLostUnitList>()
                {
                    new()
                    {
                        unit_no = 1,
                        entity_type = EntityTypes.Chara,
                        entity_id = (int)Charas.Basileus,
                    },
                    new()
                    {
                        unit_no = 2,
                        entity_type = EntityTypes.Wyrmprint,
                        entity_id = (int)AbilityCrests.InanUnendingWorld,
                    },
                    new()
                    {
                        unit_no = 3,
                        entity_type = EntityTypes.WeaponBody,
                        entity_id = (int)WeaponBodies.PrimalAqua,
                    },
                    new()
                    {
                        unit_no = 4,
                        entity_type = EntityTypes.WeaponSkin,
                        entity_id = 1000
                    },
                    new()
                    {
                        unit_no = 5,
                        entity_type = EntityTypes.Dragon,
                        entity_id = (int)Dragons.Ifrit
                    },
                    new()
                    {
                        unit_no = 6,
                        entity_type = EntityTypes.Talisman,
                        entity_id = (int)Talismans.Raemond
                    }
                }
            );

        List<PartySettingList> partyList = response.data.quest_clear_party_setting_list.ToList();

        partyList[0].chara_id.Should().Be(0);
        partyList[1].equip_crest_slot_type_1_crest_id_1.Should().Be(0);
        partyList[2].equip_weapon_body_id.Should().Be(0);
        partyList[3].equip_weapon_skin_id.Should().Be(0);
        partyList[4].equip_dragon_key_id.Should().Be(0);
        partyList[5].equip_talisman_key_id.Should().Be(0);
        partyList[6].edit_skill_1_chara_id.Should().Be(0);
        partyList[6].edit_skill_2_chara_id.Should().Be(0);
    }

    [Fact]
    public async Task SetQuestClearParty_AddsToDatabase()
    {
        await this.ImportSave();

        DragaliaResponse<QuestSetQuestClearPartyData> response =
            await this.Client.PostMsgpack<QuestSetQuestClearPartyData>(
                "/quest/set_quest_clear_party",
                new QuestSetQuestClearPartyRequest()
                {
                    quest_id = 3,
                    request_party_setting_list = SoloPartySettingLists
                }
            );

        response.data.result.Should().Be(1);

        List<DbQuestClearPartyUnit> storedList = await this.ApiContext.QuestClearPartyUnits.Where(
            x => x.QuestId == 3 && x.ViewerId == ViewerId && x.IsMulti == false
        )
            .ToListAsync();

        storedList.Should().BeEquivalentTo(SoloDbEntities, opts => opts.Excluding(x => x.QuestId));
        storedList.Should().AllSatisfy(x => x.QuestId.Should().Be(3));
    }

    [Fact]
    public async Task SetQuestClearPartyMulti_AddsToDatabase()
    {
        await this.ImportSave();

        DragaliaResponse<QuestSetQuestClearPartyData> response =
            await this.Client.PostMsgpack<QuestSetQuestClearPartyData>(
                "/quest/set_quest_clear_party_multi",
                new QuestSetQuestClearPartyRequest()
                {
                    quest_id = 4,
                    request_party_setting_list = MultiPartySettingLists
                }
            );

        response.data.result.Should().Be(1);

        List<DbQuestClearPartyUnit> storedList = await this.ApiContext.QuestClearPartyUnits.Where(
            x => x.QuestId == 4 && x.ViewerId == ViewerId && x.IsMulti == true
        )
            .ToListAsync();

        storedList.Should().BeEquivalentTo(MultiDbEntities, opts => opts.Excluding(x => x.QuestId));
        storedList.Should().AllSatisfy(x => x.QuestId.Should().Be(4));
    }

    private List<DbQuestClearPartyUnit> SoloDbEntities =>
        new()
        {
            new()
            {
                ViewerId = ViewerId,
                IsMulti = false,
                QuestId = 1,
                UnitNo = 1,
                CharaId = Charas.GalaNedrick,
                EquipDragonKeyId = GetDragonKeyId(Dragons.Cerberus),
                EquipWeaponBodyId = WeaponBodies.YitianJian,
                EquipCrestSlotType1CrestId1 = AbilityCrests.PrimalCrisis,
                EquipCrestSlotType1CrestId2 = AbilityCrests.WelcometotheOpera,
                EquipCrestSlotType1CrestId3 = AbilityCrests.FelyneHospitality,
                EquipCrestSlotType2CrestId1 = AbilityCrests.ThePlaguebringer,
                EquipCrestSlotType2CrestId2 = AbilityCrests.TotheExtreme,
                EquipCrestSlotType3CrestId1 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.TutelarysDestinyWolfsBoon,
                EquipTalismanKeyId = GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 30129901,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
                EquippedDragonEntityId = Dragons.Cerberus,
                EquippedTalismanEntityId = Talismans.GalaMym,
            },
            new()
            {
                ViewerId = ViewerId,
                IsMulti = false,
                QuestId = 1,
                UnitNo = 2,
                CharaId = Charas.Patia,
                EquipDragonKeyId = GetDragonKeyId(Dragons.Pazuzu),
                EquipWeaponBodyId = WeaponBodies.QinglongYanyuedao,
                EquipCrestSlotType1CrestId1 = AbilityCrests.AHalloweenSpectacular,
                EquipCrestSlotType1CrestId2 = AbilityCrests.CastawaysJournal,
                EquipCrestSlotType1CrestId3 = AbilityCrests.TheChocolatiers,
                EquipCrestSlotType2CrestId1 = AbilityCrests.RoguesBanquet,
                EquipCrestSlotType2CrestId2 = AbilityCrests.LuckoftheDraw,
                EquipCrestSlotType3CrestId1 = AbilityCrests.RavenousFireCrownsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.PromisedPietyStaffsBoon,
                EquipTalismanKeyId = GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 30129901,
                EditSkill1CharaId = Charas.TemplarHope,
                EditSkill2CharaId = Charas.Zena,
                EquippedDragonEntityId = Dragons.Pazuzu,
                EquippedTalismanEntityId = Talismans.GalaMym
            }
        };

    private List<PartySettingList> SoloPartySettingLists =>
        new()
        {
            new()
            {
                unit_no = 1,
                chara_id = Charas.GalaNedrick,
                equip_dragon_key_id = (ulong)GetDragonKeyId(Dragons.Cerberus),
                equip_weapon_body_id = WeaponBodies.YitianJian,
                equip_crest_slot_type_1_crest_id_1 = AbilityCrests.PrimalCrisis,
                equip_crest_slot_type_1_crest_id_2 = AbilityCrests.WelcometotheOpera,
                equip_crest_slot_type_1_crest_id_3 = AbilityCrests.FelyneHospitality,
                equip_crest_slot_type_2_crest_id_1 = AbilityCrests.ThePlaguebringer,
                equip_crest_slot_type_2_crest_id_2 = AbilityCrests.TotheExtreme,
                equip_crest_slot_type_3_crest_id_1 = AbilityCrests.CrownofLightSerpentsBoon,
                equip_crest_slot_type_3_crest_id_2 = AbilityCrests.TutelarysDestinyWolfsBoon,
                equip_talisman_key_id = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                equip_weapon_skin_id = 30129901,
                edit_skill_1_chara_id = Charas.Empty,
                edit_skill_2_chara_id = Charas.GalaMym,
            },
            new()
            {
                unit_no = 2,
                chara_id = Charas.Patia,
                equip_dragon_key_id = (ulong)GetDragonKeyId(Dragons.Pazuzu),
                equip_weapon_body_id = WeaponBodies.QinglongYanyuedao,
                equip_crest_slot_type_1_crest_id_1 = AbilityCrests.AHalloweenSpectacular,
                equip_crest_slot_type_1_crest_id_2 = AbilityCrests.CastawaysJournal,
                equip_crest_slot_type_1_crest_id_3 = AbilityCrests.TheChocolatiers,
                equip_crest_slot_type_2_crest_id_1 = AbilityCrests.RoguesBanquet,
                equip_crest_slot_type_2_crest_id_2 = AbilityCrests.LuckoftheDraw,
                equip_crest_slot_type_3_crest_id_1 = AbilityCrests.RavenousFireCrownsBoon,
                equip_crest_slot_type_3_crest_id_2 = AbilityCrests.PromisedPietyStaffsBoon,
                equip_talisman_key_id = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                equip_weapon_skin_id = 30129901,
                edit_skill_1_chara_id = Charas.TemplarHope,
                edit_skill_2_chara_id = Charas.Zena,
            }
        };

    private List<DbQuestClearPartyUnit> MultiDbEntities =>
        new()
        {
            new()
            {
                ViewerId = ViewerId,
                IsMulti = true,
                QuestId = 2,
                UnitNo = 1,
                CharaId = Charas.GalaNotte,
                EquipDragonKeyId = GetDragonKeyId(Dragons.Leviathan),
                EquipWeaponBodyId = WeaponBodies.WindrulersFang,
                EquipCrestSlotType1CrestId1 = AbilityCrests.BondsBetweenWorlds,
                EquipCrestSlotType1CrestId2 = AbilityCrests.AManUnchanging,
                EquipCrestSlotType1CrestId3 = AbilityCrests.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrests.APassionforProduce,
                EquipCrestSlotType2CrestId2 = AbilityCrests.DragonsNest,
                EquipCrestSlotType3CrestId1 = AbilityCrests.TutelarysDestinyWolfsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 0,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
                EquippedDragonEntityId = Dragons.Leviathan,
                EquippedTalismanEntityId = Talismans.GalaMym
            },
            new()
            {
                ViewerId = ViewerId,
                IsMulti = true,
                QuestId = 2,
                UnitNo = 2,
                CharaId = Charas.GalaLeif,
                EquipDragonKeyId = GetDragonKeyId(Dragons.Phoenix),
                EquipWeaponBodyId = WeaponBodies.PrimalTempest,
                EquipCrestSlotType1CrestId1 = AbilityCrests.AdventureinthePast,
                EquipCrestSlotType1CrestId2 = AbilityCrests.PrimalCrisis,
                EquipCrestSlotType1CrestId3 = AbilityCrests.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrests.DragonsNest,
                EquipCrestSlotType2CrestId2 = AbilityCrests.ThePlaguebringer,
                EquipCrestSlotType3CrestId1 = AbilityCrests.AKnightsDreamAxesBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 0,
                EditSkill1CharaId = Charas.ShaWujing,
                EditSkill2CharaId = Charas.Ranzal,
                EquippedDragonEntityId = Dragons.Phoenix,
                EquippedTalismanEntityId = Talismans.GalaMym
            }
        };

    private List<PartySettingList> MultiPartySettingLists =>
        new()
        {
            new()
            {
                unit_no = 1,
                chara_id = Charas.GalaNotte,
                equip_dragon_key_id = (ulong)GetDragonKeyId(Dragons.Leviathan),
                equip_weapon_body_id = WeaponBodies.WindrulersFang,
                equip_crest_slot_type_1_crest_id_1 = AbilityCrests.BondsBetweenWorlds,
                equip_crest_slot_type_1_crest_id_2 = AbilityCrests.AManUnchanging,
                equip_crest_slot_type_1_crest_id_3 = AbilityCrests.GoingUndercover,
                equip_crest_slot_type_2_crest_id_1 = AbilityCrests.APassionforProduce,
                equip_crest_slot_type_2_crest_id_2 = AbilityCrests.DragonsNest,
                equip_crest_slot_type_3_crest_id_1 = AbilityCrests.TutelarysDestinyWolfsBoon,
                equip_crest_slot_type_3_crest_id_2 = AbilityCrests.CrownofLightSerpentsBoon,
                equip_talisman_key_id = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                equip_weapon_skin_id = 0,
                edit_skill_1_chara_id = Charas.Empty,
                edit_skill_2_chara_id = Charas.GalaMym,
            },
            new()
            {
                unit_no = 2,
                chara_id = Charas.GalaLeif,
                equip_dragon_key_id = (ulong)GetDragonKeyId(Dragons.Phoenix),
                equip_weapon_body_id = WeaponBodies.PrimalTempest,
                equip_crest_slot_type_1_crest_id_1 = AbilityCrests.AdventureinthePast,
                equip_crest_slot_type_1_crest_id_2 = AbilityCrests.PrimalCrisis,
                equip_crest_slot_type_1_crest_id_3 = AbilityCrests.GoingUndercover,
                equip_crest_slot_type_2_crest_id_1 = AbilityCrests.DragonsNest,
                equip_crest_slot_type_2_crest_id_2 = AbilityCrests.ThePlaguebringer,
                equip_crest_slot_type_3_crest_id_1 = AbilityCrests.AKnightsDreamAxesBoon,
                equip_crest_slot_type_3_crest_id_2 = AbilityCrests.CrownofLightSerpentsBoon,
                equip_talisman_key_id = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                equip_weapon_skin_id = 0,
                edit_skill_1_chara_id = Charas.ShaWujing,
                edit_skill_2_chara_id = Charas.Ranzal,
            }
        };

    private List<DbQuestClearPartyUnit> MissingItemDbEntities =>
        new()
        {
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 1,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Basileus,
            },
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 2,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Cecile,
                EquipCrestSlotType1CrestId1 = AbilityCrests.InanUnendingWorld
            },
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 3,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Durant,
                EquipWeaponBodyId = WeaponBodies.PrimalAqua,
            },
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 4,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Elias,
                EquipWeaponSkinId = 1000,
            },
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 5,
                QuestId = 6,
                IsMulti = false,
                EquipDragonKeyId = 2000,
                CharaId = Charas.Emma,
                EquippedDragonEntityId = Dragons.Ifrit,
            },
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 6,
                QuestId = 6,
                IsMulti = false,
                EquipTalismanKeyId = 3000,
                CharaId = Charas.Raemond,
                EquippedTalismanEntityId = Talismans.Raemond
            },
            new()
            {
                ViewerId = ViewerId,
                UnitNo = 7,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Edward,
                EditSkill1CharaId = Charas.Yue,
                EditSkill2CharaId = Charas.Marty
            }
        };
}
