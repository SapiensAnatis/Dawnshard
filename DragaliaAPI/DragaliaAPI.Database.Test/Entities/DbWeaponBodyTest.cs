using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Test.Entities;

public class DbWeaponBodyTest
{
    [Theory]
    [InlineData(WeaponBodies.Nothung, 0, 0)]
    [InlineData(WeaponBodies.Nothung, 1, 2)]
    [InlineData(WeaponBodies.Nothung, 2, 3)]
    [InlineData(WeaponBodies.PrimalCrimson, 0, 1)]
    [InlineData(WeaponBodies.PrimalCrimson, 1, 2)]
    [InlineData(WeaponBodies.ChimeratechCommander, 0, 1)]
    [InlineData(WeaponBodies.ChimeratechCommander, 1, 2)]
    [InlineData(WeaponBodies.InfernoApogee, 0, 1)]
    [InlineData(WeaponBodies.InfernoApogee, 1, 2)]
    public void AbilityOneResolver_ReturnsExpectedLevel(
        WeaponBodies id,
        int limitOverCount,
        int expectedLevel
    )
    {
        DbWeaponBody weapon = new() { WeaponBodyId = id, LimitOverCount = limitOverCount, };

        weapon.Ability1Level.Should().Be(expectedLevel);
    }

    [Theory]
    [InlineData(WeaponBodies.Nothung, 0, 0)]
    [InlineData(WeaponBodies.Nothung, 1, 2)]
    [InlineData(WeaponBodies.Nothung, 2, 3)]
    [InlineData(WeaponBodies.PrimalCrimson, 0, 0)]
    [InlineData(WeaponBodies.PrimalCrimson, 1, 0)]
    [InlineData(WeaponBodies.ChimeratechCommander, 0, 0)]
    [InlineData(WeaponBodies.ChimeratechCommander, 1, 0)]
    [InlineData(WeaponBodies.InfernoApogee, 0, 0)]
    [InlineData(WeaponBodies.InfernoApogee, 1, 0)]
    public void AbilityTwoResolver_ReturnsExpectedLevel(
        WeaponBodies id,
        int limitOverCount,
        int expectedLevel
    )
    {
        DbWeaponBody weapon = new() { WeaponBodyId = id, LimitOverCount = limitOverCount, };

        weapon.Ability2Level.Should().Be(expectedLevel);
    }

    [Theory]
    [InlineData(WeaponBodies.Camelot, 2, 9, 2)]
    [InlineData(WeaponBodies.Camelot, 2, 6, 2)]
    [InlineData(WeaponBodies.Camelot, 2, 4, 2)]
    [InlineData(WeaponBodies.Camelot, 1, 6, 2)]
    [InlineData(WeaponBodies.Camelot, 1, 4, 2)]
    [InlineData(WeaponBodies.Camelot, 0, 4, 2)]
    [InlineData(WeaponBodies.Camelot, 0, 2, 1)]
    [InlineData(WeaponBodies.Camelot, 0, 0, 1)]
    [InlineData(WeaponBodies.AbsoluteCrimson, 0, 1, 1)]
    [InlineData(WeaponBodies.AbsoluteCrimson, 0, 4, 2)]
    [InlineData(WeaponBodies.AbsoluteCrimson, 1, 4, 1)]
    [InlineData(WeaponBodies.AbsoluteCrimson, 1, 6, 1)]
    [InlineData(WeaponBodies.AbsoluteCrimson, 1, 8, 2)]
    public void SkillLevelResolver_ReturnsExpectedLevel(
        WeaponBodies id,
        int limitOverCount,
        int limitBreakCount,
        int expectedLevel
    )
    {
        DbWeaponBody weapon =
            new()
            {
                WeaponBodyId = id,
                LimitBreakCount = limitBreakCount,
                LimitOverCount = limitOverCount,
            };

        weapon.SkillLevel.Should().Be(expectedLevel);
    }
}
