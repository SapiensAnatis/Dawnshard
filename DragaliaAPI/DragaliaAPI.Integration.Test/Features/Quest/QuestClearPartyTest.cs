﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Quest;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Quest;

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

        await this.AddRangeToDatabase(this.SoloDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyResponse> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyResponse>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { QuestId = 1 }
            );

        response
            .Data.QuestClearPartySettingList.Should()
            .BeEquivalentTo(this.SoloPartySettingLists);
        response.Data.LostUnitList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearPartyMulti_ReturnsSetClearParty()
    {
        await this.ImportSave();

        await this.AddRangeToDatabase(this.MultiDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyMultiResponse> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyMultiResponse>(
                "/quest/get_quest_clear_party_multi",
                new QuestGetQuestClearPartyRequest() { QuestId = 2 }
            );

        response
            .Data.QuestMultiClearPartySettingList.Should()
            .BeEquivalentTo(this.MultiPartySettingLists);
        response.Data.LostUnitList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearParty_DoesNotRetrieveMultiData()
    {
        int questId = 5;

        await this.Client.PostMsgpack<QuestSetQuestClearPartyResponse>(
            "/quest/set_quest_clear_party_multi",
            new QuestSetQuestClearPartyRequest()
            {
                QuestId = questId,
                RequestPartySettingList = this.MultiPartySettingLists
            }
        );

        DragaliaResponse<QuestGetQuestClearPartyResponse> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyResponse>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { QuestId = questId }
            );

        response.Data.QuestClearPartySettingList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearParty_HandlesMissingEntities()
    {
        await this.ImportSave();

        int questId = this.MissingItemDbEntities[0].QuestId;

        await this.AddRangeToDatabase(this.MissingItemDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyResponse> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyResponse>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { QuestId = questId }
            );

        response
            .Data.LostUnitList.Should()
            .BeEquivalentTo(
                new List<AtgenLostUnitList>()
                {
                    new()
                    {
                        UnitNo = 1,
                        EntityType = EntityTypes.Chara,
                        EntityId = (int)Charas.Basileus,
                    },
                    new()
                    {
                        UnitNo = 2,
                        EntityType = EntityTypes.Wyrmprint,
                        EntityId = (int)AbilityCrestId.InanUnendingWorld,
                    },
                    new()
                    {
                        UnitNo = 3,
                        EntityType = EntityTypes.WeaponBody,
                        EntityId = (int)WeaponBodies.PrimalAqua,
                    },
                    new()
                    {
                        UnitNo = 4,
                        EntityType = EntityTypes.WeaponSkin,
                        EntityId = 1000
                    },
                    new()
                    {
                        UnitNo = 5,
                        EntityType = EntityTypes.Dragon,
                        EntityId = (int)Dragons.Ifrit
                    },
                    new()
                    {
                        UnitNo = 6,
                        EntityType = EntityTypes.Talisman,
                        EntityId = (int)Talismans.Raemond
                    }
                }
            );

        List<PartySettingList> partyList = response.Data.QuestClearPartySettingList.ToList();

        partyList[0].CharaId.Should().Be(0);
        partyList[1].EquipCrestSlotType1CrestId1.Should().Be(0);
        partyList[2].EquipWeaponBodyId.Should().Be(0);
        partyList[3].EquipWeaponSkinId.Should().Be(0);
        partyList[4].EquipDragonKeyId.Should().Be(0);
        partyList[5].EquipTalismanKeyId.Should().Be(0);
        partyList[6].EditSkill1CharaId.Should().Be(0);
        partyList[6].EditSkill2CharaId.Should().Be(0);
    }

    [Fact]
    public async Task SetQuestClearParty_AddsToDatabase()
    {
        await this.ImportSave();

        DragaliaResponse<QuestSetQuestClearPartyResponse> response =
            await this.Client.PostMsgpack<QuestSetQuestClearPartyResponse>(
                "/quest/set_quest_clear_party",
                new QuestSetQuestClearPartyRequest()
                {
                    QuestId = 3,
                    RequestPartySettingList = this.SoloPartySettingLists
                }
            );

        response.Data.Result.Should().Be(1);

        List<DbQuestClearPartyUnit> storedList = await this
            .ApiContext.QuestClearPartyUnits.Where(x =>
                x.QuestId == 3 && x.ViewerId == this.ViewerId && x.IsMulti == false
            )
            .ToListAsync();

        storedList
            .Should()
            .BeEquivalentTo(this.SoloDbEntities, opts => opts.Excluding(x => x.QuestId));
        storedList.Should().AllSatisfy(x => x.QuestId.Should().Be(3));
    }

    [Fact]
    public async Task SetQuestClearPartyMulti_AddsToDatabase()
    {
        await this.ImportSave();

        DragaliaResponse<QuestSetQuestClearPartyResponse> response =
            await this.Client.PostMsgpack<QuestSetQuestClearPartyResponse>(
                "/quest/set_quest_clear_party_multi",
                new QuestSetQuestClearPartyRequest()
                {
                    QuestId = 4,
                    RequestPartySettingList = this.MultiPartySettingLists
                }
            );

        response.Data.Result.Should().Be(1);

        List<DbQuestClearPartyUnit> storedList = await this
            .ApiContext.QuestClearPartyUnits.Where(x =>
                x.QuestId == 4 && x.ViewerId == this.ViewerId && x.IsMulti == true
            )
            .ToListAsync();

        storedList
            .Should()
            .BeEquivalentTo(this.MultiDbEntities, opts => opts.Excluding(x => x.QuestId));
        storedList.Should().AllSatisfy(x => x.QuestId.Should().Be(4));
    }

    private List<DbQuestClearPartyUnit> SoloDbEntities =>
        new()
        {
            new()
            {
                ViewerId = this.ViewerId,
                IsMulti = false,
                QuestId = 1,
                UnitNo = 1,
                CharaId = Charas.GalaNedrick,
                EquipDragonKeyId = this.GetDragonKeyId(Dragons.Cerberus),
                EquipWeaponBodyId = WeaponBodies.YitianJian,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.PrimalCrisis,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.WelcometotheOpera,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.FelyneHospitality,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.ThePlaguebringer,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.TotheExtreme,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.CrownofLightSerpentsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                EquipTalismanKeyId = this.GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 30129901,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
                EquippedDragonEntityId = Dragons.Cerberus,
                EquippedTalismanEntityId = Talismans.GalaMym,
            },
            new()
            {
                ViewerId = this.ViewerId,
                IsMulti = false,
                QuestId = 1,
                UnitNo = 2,
                CharaId = Charas.Patia,
                EquipDragonKeyId = this.GetDragonKeyId(Dragons.Pazuzu),
                EquipWeaponBodyId = WeaponBodies.QinglongYanyuedao,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.AHalloweenSpectacular,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.CastawaysJournal,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.TheChocolatiers,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.RoguesBanquet,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.LuckoftheDraw,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.RavenousFireCrownsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.PromisedPietyStaffsBoon,
                EquipTalismanKeyId = this.GetTalismanKeyId(Talismans.GalaMym),
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
                UnitNo = 1,
                CharaId = Charas.GalaNedrick,
                EquipDragonKeyId = (ulong)this.GetDragonKeyId(Dragons.Cerberus),
                EquipWeaponBodyId = WeaponBodies.YitianJian,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.PrimalCrisis,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.WelcometotheOpera,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.FelyneHospitality,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.ThePlaguebringer,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.TotheExtreme,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.CrownofLightSerpentsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                EquipTalismanKeyId = (ulong)this.GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 30129901,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
            },
            new()
            {
                UnitNo = 2,
                CharaId = Charas.Patia,
                EquipDragonKeyId = (ulong)this.GetDragonKeyId(Dragons.Pazuzu),
                EquipWeaponBodyId = WeaponBodies.QinglongYanyuedao,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.AHalloweenSpectacular,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.CastawaysJournal,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.TheChocolatiers,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.RoguesBanquet,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.LuckoftheDraw,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.RavenousFireCrownsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.PromisedPietyStaffsBoon,
                EquipTalismanKeyId = (ulong)this.GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 30129901,
                EditSkill1CharaId = Charas.TemplarHope,
                EditSkill2CharaId = Charas.Zena,
            }
        };

    private List<DbQuestClearPartyUnit> MultiDbEntities =>
        new()
        {
            new()
            {
                ViewerId = this.ViewerId,
                IsMulti = true,
                QuestId = 2,
                UnitNo = 1,
                CharaId = Charas.GalaNotte,
                EquipDragonKeyId = this.GetDragonKeyId(Dragons.Leviathan),
                EquipWeaponBodyId = WeaponBodies.WindrulersFang,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.BondsBetweenWorlds,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.AManUnchanging,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.APassionforProduce,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.DragonsNest,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = this.GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 0,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
                EquippedDragonEntityId = Dragons.Leviathan,
                EquippedTalismanEntityId = Talismans.GalaMym
            },
            new()
            {
                ViewerId = this.ViewerId,
                IsMulti = true,
                QuestId = 2,
                UnitNo = 2,
                CharaId = Charas.GalaLeif,
                EquipDragonKeyId = this.GetDragonKeyId(Dragons.Phoenix),
                EquipWeaponBodyId = WeaponBodies.PrimalTempest,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.AdventureinthePast,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.PrimalCrisis,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.DragonsNest,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.ThePlaguebringer,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.AKnightsDreamAxesBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = this.GetTalismanKeyId(Talismans.GalaMym),
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
                UnitNo = 1,
                CharaId = Charas.GalaNotte,
                EquipDragonKeyId = (ulong)this.GetDragonKeyId(Dragons.Leviathan),
                EquipWeaponBodyId = WeaponBodies.WindrulersFang,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.BondsBetweenWorlds,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.AManUnchanging,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.APassionforProduce,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.DragonsNest,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = (ulong)this.GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 0,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
            },
            new()
            {
                UnitNo = 2,
                CharaId = Charas.GalaLeif,
                EquipDragonKeyId = (ulong)this.GetDragonKeyId(Dragons.Phoenix),
                EquipWeaponBodyId = WeaponBodies.PrimalTempest,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.AdventureinthePast,
                EquipCrestSlotType1CrestId2 = AbilityCrestId.PrimalCrisis,
                EquipCrestSlotType1CrestId3 = AbilityCrestId.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrestId.DragonsNest,
                EquipCrestSlotType2CrestId2 = AbilityCrestId.ThePlaguebringer,
                EquipCrestSlotType3CrestId1 = AbilityCrestId.AKnightsDreamAxesBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = (ulong)this.GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 0,
                EditSkill1CharaId = Charas.ShaWujing,
                EditSkill2CharaId = Charas.Ranzal,
            }
        };

    private List<DbQuestClearPartyUnit> MissingItemDbEntities =>
        new()
        {
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 1,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Basileus,
            },
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 2,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Cecile,
                EquipCrestSlotType1CrestId1 = AbilityCrestId.InanUnendingWorld
            },
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 3,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Durant,
                EquipWeaponBodyId = WeaponBodies.PrimalAqua,
            },
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 4,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Elias,
                EquipWeaponSkinId = 1000,
            },
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 5,
                QuestId = 6,
                IsMulti = false,
                EquipDragonKeyId = long.MaxValue,
                CharaId = Charas.Emma,
                EquippedDragonEntityId = Dragons.Ifrit,
            },
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 6,
                QuestId = 6,
                IsMulti = false,
                EquipTalismanKeyId = 3000,
                CharaId = Charas.Raemond,
                EquippedTalismanEntityId = Talismans.Raemond
            },
            new()
            {
                ViewerId = this.ViewerId,
                UnitNo = 7,
                QuestId = 6,
                IsMulti = false,
                CharaId = Charas.Edward,
                EditSkill1CharaId = Charas.Yue,
                EditSkill2CharaId = Charas.Marty
            }
        };
}
