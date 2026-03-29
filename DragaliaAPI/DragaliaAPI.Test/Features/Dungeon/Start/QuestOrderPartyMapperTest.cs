using DragaliaAPI.Features.Dungeon.Start;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Test.Features.Dungeon.Start;

public class QuestOrderPartyMapperTest
{
    [Fact]
    public void MapToPartyUnitList_SingleUnit_ReturnsExpectedPartyPaddedToFour()
    {
        List<QuestOrderParty> orderPartyUnits = MasterAsset
            .QuestOrderParty.Enumerable.Where(x => x.QuestOrderPartyGroupId == 1002601)
            .ToList();

        List<PartyUnitList> result = QuestOrderPartyMapper.MapToPartyUnitList(orderPartyUnits);

        result.Should().HaveCount(4);

        PartyUnitList unit1 = result[0];
        unit1.Position.Should().Be(1);
        unit1.CharaData.CharaId.Should().Be((Charas)15150306);
        unit1.CharaData.Level.Should().Be(80);
        unit1.CharaData.Rarity.Should().Be(5);
        unit1.CharaData.LimitBreakCount.Should().Be(4);
        unit1.DragonData.DragonId.Should().Be((DragonId)20040301);
        unit1.DragonData.Level.Should().Be(80);
        unit1.DragonData.LimitBreakCount.Should().Be(4);
        unit1.DragonData.Skill1Level.Should().Be(2);
        unit1.DragonData.Ability1Level.Should().Be(5);
        unit1.DragonData.Ability2Level.Should().Be(5);
        unit1.WeaponBodyData.WeaponBodyId.Should().Be((WeaponBodies)30160304);
        unit1.WeaponBodyData.BuildupCount.Should().Be(80);
        unit1.WeaponBodyData.LimitBreakCount.Should().Be(8);
        unit1.WeaponBodyData.LimitOverCount.Should().Be(1);
        unit1.WeaponSkinData.WeaponSkinId.Should().Be(30159950);

        // Remaining slots should be empty padding
        for (int i = 1; i < 4; i++)
        {
            result[i].Position.Should().Be(i + 1);
            result[i].CharaData.CharaId.Should().Be(Charas.Empty);
        }
    }

    [Fact]
    public void MapToPartyUnitList_FourUnits_ReturnsAllFourUnits()
    {
        List<QuestOrderParty> orderPartyUnits = MasterAsset
            .QuestOrderParty.Enumerable.Where(x => x.QuestOrderPartyGroupId == 2042902)
            .ToList();

        List<PartyUnitList> result = QuestOrderPartyMapper.MapToPartyUnitList(orderPartyUnits);

        result.Should().HaveCount(4);
        result[0].CharaData.CharaId.Should().Be((Charas)10350505);
        result[1].CharaData.CharaId.Should().Be((Charas)10450404);
        result[2].CharaData.CharaId.Should().Be((Charas)10150304);
        result[3].CharaData.CharaId.Should().Be((Charas)10550104);

        for (int i = 0; i < 4; i++)
        {
            result[i].Position.Should().Be(i + 1);
        }
    }

    [Fact]
    public void MapToPartyUnitList_CharaAbilityLevels_DerivedFromMasterData()
    {
        List<QuestOrderParty> orderPartyUnits = MasterAsset
            .QuestOrderParty.Enumerable.Where(x => x.QuestOrderPartyGroupId == 1002601)
            .ToList();

        List<PartyUnitList> result = QuestOrderPartyMapper.MapToPartyUnitList(orderPartyUnits);

        CharaData charaData = MasterAsset.CharaData[(Charas)15150306];
        CharaList chara = result[0].CharaData;

        chara.Ability1Level.Should().Be(charaData.MaxAbility1Level);
        chara.Ability2Level.Should().Be(charaData.MaxAbility2Level);
        chara.Ability3Level.Should().Be(charaData.MaxAbility3Level);
        chara.Hp.Should().Be(charaData.MaxHp);
        chara.Attack.Should().Be(charaData.MaxAtk);
        chara.BurstAttackLevel.Should().Be(charaData.DefaultBurstAttackLevel);
    }

    [Fact]
    public void MapToPartyUnitList_DragonReliabilityLevel_DerivedFromMasterData()
    {
        List<QuestOrderParty> orderPartyUnits = MasterAsset
            .QuestOrderParty.Enumerable.Where(x => x.QuestOrderPartyGroupId == 1002601)
            .ToList();

        List<PartyUnitList> result = QuestOrderPartyMapper.MapToPartyUnitList(orderPartyUnits);

        DragonData dragonData = MasterAsset.DragonData[(DragonId)20040301];
        result[0].DragonReliabilityLevel.Should().Be(30);
    }

    [Fact]
    public void MapToPartySettingList_ReturnsExpectedEntries()
    {
        List<QuestOrderParty> orderPartyUnits = MasterAsset
            .QuestOrderParty.Enumerable.Where(x => x.QuestOrderPartyGroupId == 2042902)
            .ToList();

        List<PartySettingList> result = QuestOrderPartyMapper.MapToPartySettingList(
            orderPartyUnits
        );

        result.Should().HaveCount(4);
        result[0].UnitNo.Should().Be(1);
        result[0].CharaId.Should().Be((Charas)10350505);
        result[0].EquipWeaponBodyId.Should().Be((WeaponBodies)30359901);
        result[3].UnitNo.Should().Be(4);
        result[3].CharaId.Should().Be((Charas)10550104);
        result[3].EquipWeaponBodyId.Should().Be((WeaponBodies)30559901);
    }

    [Fact]
    public void MapToPartyUnitList_AllExistingEntries_DoNotThrow()
    {
        // Verify all existing QuestOrderParty groups can be mapped without throwing
        IEnumerable<int> groupIds = MasterAsset
            .QuestOrderParty.Enumerable.Select(x => x.QuestOrderPartyGroupId)
            .Distinct();

        foreach (int groupId in groupIds)
        {
            List<QuestOrderParty> units = MasterAsset
                .QuestOrderParty.Enumerable.Where(x => x.QuestOrderPartyGroupId == groupId)
                .ToList();

            Action act = () => QuestOrderPartyMapper.MapToPartyUnitList(units);
            act.Should().NotThrow($"group {groupId} should be mappable");
        }
    }
}
