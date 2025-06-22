using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Test.Features.Dungeon;

public class AbilityLogicTest
{
    [Fact]
    public void CalculateEventMultipliers_ReturnsExpectedResult()
    {
        int flamesOfReflectionCompendiumId = 20816;

        List<List<int>> abilityIds =
        [
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // 150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TheDragonSmiths].GetAbilities(3), // 75% materials
            ],
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // 150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TheDragonSmiths].GetAbilities(3), // 75% materials
            ],
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // 150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TheDragonSmiths].GetAbilities(3), // 75% materials
            ],
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // 150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TheDragonSmiths].GetAbilities(3), // 75% materials
            ],
        ];

        double point = AbilityLogic.CalculateEventPointMultiplier(
            abilityIds,
            flamesOfReflectionCompendiumId
        );
        double material = AbilityLogic.CalculateEventMaterialMultiplier(
            abilityIds,
            flamesOfReflectionCompendiumId
        );

        point.Should().Be(7); // +600%
        material.Should().Be(4); // +300%
    }

    [Fact]
    public void CalculateEventPointMultiplier_UnitOverCap_ReturnsBoostCap()
    {
        int flamesOfReflectionCompendiumId = 20816;

        List<List<int>> abilityIds =
        [
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // +150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TotheExtreme].GetAbilities(3), // +100% points
            ],
        ];

        double point = AbilityLogic.CalculateEventPointMultiplier(
            abilityIds,
            flamesOfReflectionCompendiumId
        );

        point.Should().Be(2.5); // +150%, cap is 150%
    }

    [Fact]
    public void CalculateEventPointMultiplier_MultipleUnitsOverCap_CapsPerUnit()
    {
        int flamesOfReflectionCompendiumId = 20816;

        List<List<int>> abilityIds =
        [
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // +150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TotheExtreme].GetAbilities(3), // +100% points
            ],
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(3), // +150% points
                .. MasterAsset.AbilityCrest[AbilityCrestId.TotheExtreme].GetAbilities(3), // +100% points
            ],
        ];

        double point = AbilityLogic.CalculateEventPointMultiplier(
            abilityIds,
            flamesOfReflectionCompendiumId
        );

        point.Should().Be(4); // +300%, cap is 150%
    }

    [Fact]
    public void CalculateEventPointMultiplier_AbilityCrestNotMaxed_UsesCorrectValues()
    {
        int flamesOfReflectionCompendiumId = 20816;

        List<List<int>> abilityIds =
        [
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(1), // +100% points
            ],
        ];

        double point = AbilityLogic.CalculateEventPointMultiplier(
            abilityIds,
            flamesOfReflectionCompendiumId
        );

        point.Should().Be(2); // +100%
    }

    [Fact]
    public void CalculateEventPointMultiplier_NoneEquippedForEvent_ReturnsNoMultiplier()
    {
        int accursedArchivesCompendiumId = 20831;

        List<List<int>> abilityIds =
        [
            [
                .. MasterAsset.AbilityCrest[AbilityCrestId.SistersDayOut].GetAbilities(1), // +100% points
            ],
        ];

        double point = AbilityLogic.CalculateEventPointMultiplier(
            abilityIds,
            accursedArchivesCompendiumId
        );

        point.Should().Be(1);
    }

    [Fact]
    public void CalculateCoinMultiplier_ReturnsExpectedResult()
    {
        List<List<int>> abilityIds =
        [
            [
                MasterAsset.DragonData[DragonId.GoldFafnir].GetAbility(1, 1), // +25% rupies
                MasterAsset.DragonData[DragonId.GoldFafnir].GetAbility(1, 2), // +30% rupies
                MasterAsset.DragonData[DragonId.GoldFafnir].GetAbility(1, 3), // +35% rupies
                MasterAsset.DragonData[DragonId.GoldFafnir].GetAbility(1, 4), // +40% rupies
            ],
        ];

        double point = AbilityLogic.CalculateCoinMultiplier(abilityIds);

        point.Should().Be(2.3); // +130%
    }
}
