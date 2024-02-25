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

        DragaliaResponse<QuestGetQuestClearPartyResponse> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyResponse>(
                "/quest/get_quest_clear_party",
                new QuestGetQuestClearPartyRequest() { QuestId = 1 }
            );

        response.Data.QuestClearPartySettingList.Should().BeEquivalentTo(SoloPartySettingLists);
        response.Data.LostUnitList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuestClearPartyMulti_ReturnsSetClearParty()
    {
        await this.ImportSave();

        await this.AddRangeToDatabase(MultiDbEntities);

        DragaliaResponse<QuestGetQuestClearPartyMultiResponse> response =
            await this.Client.PostMsgpack<QuestGetQuestClearPartyMultiResponse>(
                "/quest/get_quest_clear_party_multi",
                new QuestGetQuestClearPartyRequest() { QuestId = 2 }
            );

        response
            .Data.QuestMultiClearPartySettingList.Should()
            .BeEquivalentTo(MultiPartySettingLists);
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
                RequestPartySettingList = MultiPartySettingLists
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

        int questId = MissingItemDbEntities[0].QuestId;

        await this.AddRangeToDatabase(MissingItemDbEntities);

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
                        EntityId = (int)AbilityCrests.InanUnendingWorld,
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
                    RequestPartySettingList = SoloPartySettingLists
                }
            );

        response.Data.Result.Should().Be(1);

        List<DbQuestClearPartyUnit> storedList = await this
            .ApiContext.QuestClearPartyUnits.Where(x =>
                x.QuestId == 3 && x.ViewerId == ViewerId && x.IsMulti == false
            )
            .ToListAsync();

        storedList.Should().BeEquivalentTo(SoloDbEntities, opts => opts.Excluding(x => x.QuestId));
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
                    RequestPartySettingList = MultiPartySettingLists
                }
            );

        response.Data.Result.Should().Be(1);

        List<DbQuestClearPartyUnit> storedList = await this
            .ApiContext.QuestClearPartyUnits.Where(x =>
                x.QuestId == 4 && x.ViewerId == ViewerId && x.IsMulti == true
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
                UnitNo = 1,
                CharaId = Charas.GalaNedrick,
                EquipDragonKeyId = (ulong)GetDragonKeyId(Dragons.Cerberus),
                EquipWeaponBodyId = WeaponBodies.YitianJian,
                EquipCrestSlotType1CrestId1 = AbilityCrests.PrimalCrisis,
                EquipCrestSlotType1CrestId2 = AbilityCrests.WelcometotheOpera,
                EquipCrestSlotType1CrestId3 = AbilityCrests.FelyneHospitality,
                EquipCrestSlotType2CrestId1 = AbilityCrests.ThePlaguebringer,
                EquipCrestSlotType2CrestId2 = AbilityCrests.TotheExtreme,
                EquipCrestSlotType3CrestId1 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.TutelarysDestinyWolfsBoon,
                EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 30129901,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
            },
            new()
            {
                UnitNo = 2,
                CharaId = Charas.Patia,
                EquipDragonKeyId = (ulong)GetDragonKeyId(Dragons.Pazuzu),
                EquipWeaponBodyId = WeaponBodies.QinglongYanyuedao,
                EquipCrestSlotType1CrestId1 = AbilityCrests.AHalloweenSpectacular,
                EquipCrestSlotType1CrestId2 = AbilityCrests.CastawaysJournal,
                EquipCrestSlotType1CrestId3 = AbilityCrests.TheChocolatiers,
                EquipCrestSlotType2CrestId1 = AbilityCrests.RoguesBanquet,
                EquipCrestSlotType2CrestId2 = AbilityCrests.LuckoftheDraw,
                EquipCrestSlotType3CrestId1 = AbilityCrests.RavenousFireCrownsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.PromisedPietyStaffsBoon,
                EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaMym),
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
                UnitNo = 1,
                CharaId = Charas.GalaNotte,
                EquipDragonKeyId = (ulong)GetDragonKeyId(Dragons.Leviathan),
                EquipWeaponBodyId = WeaponBodies.WindrulersFang,
                EquipCrestSlotType1CrestId1 = AbilityCrests.BondsBetweenWorlds,
                EquipCrestSlotType1CrestId2 = AbilityCrests.AManUnchanging,
                EquipCrestSlotType1CrestId3 = AbilityCrests.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrests.APassionforProduce,
                EquipCrestSlotType2CrestId2 = AbilityCrests.DragonsNest,
                EquipCrestSlotType3CrestId1 = AbilityCrests.TutelarysDestinyWolfsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                EquipWeaponSkinId = 0,
                EditSkill1CharaId = Charas.Empty,
                EditSkill2CharaId = Charas.GalaMym,
            },
            new()
            {
                UnitNo = 2,
                CharaId = Charas.GalaLeif,
                EquipDragonKeyId = (ulong)GetDragonKeyId(Dragons.Phoenix),
                EquipWeaponBodyId = WeaponBodies.PrimalTempest,
                EquipCrestSlotType1CrestId1 = AbilityCrests.AdventureinthePast,
                EquipCrestSlotType1CrestId2 = AbilityCrests.PrimalCrisis,
                EquipCrestSlotType1CrestId3 = AbilityCrests.GoingUndercover,
                EquipCrestSlotType2CrestId1 = AbilityCrests.DragonsNest,
                EquipCrestSlotType2CrestId2 = AbilityCrests.ThePlaguebringer,
                EquipCrestSlotType3CrestId1 = AbilityCrests.AKnightsDreamAxesBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaMym),
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
