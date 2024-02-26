using DragaliaAPI.Features.TimeAttack.Validation;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentValidation.TestHelper;

namespace DragaliaAPI.Test.Features.TimeAttack;

public class PartyInfoValidatorTest
{
    private const int QuestId = 227010104; // Volk's Wrath Solo TA
    private PartyInfoValidator validator;

    public PartyInfoValidatorTest()
    {
        this.validator = new(QuestId);
    }

    [Fact]
    public void ValidPartyData_ReturnsSuccess()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            PartyInfoValidatorTestData.Valid
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidPartyData_DuplicateCharacters_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { CharaData = new() { CharaId = Charas.GalaGatov } },
                    new() { CharaData = new() { CharaId = Charas.GalaGatov } }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor(x => x.PartyUnitList);
    }

    [Fact]
    public void InvalidPartyData_DuplicateSharedSkills_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        CharaData = new() { CharaId = Charas.GalaGatov },
                        EditSkill1CharaData = new() { CharaId = Charas.ShaWujing },
                        EditSkill2CharaData = new() { CharaId = Charas.ShaWujing }
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0]")
            .WithErrorMessage("Duplicate shared skills");
    }

    [Fact]
    public void InvalidPartyData_InvalidDragonReliability_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        CharaData = new() { CharaId = Charas.GalaGatov },
                        DragonReliabilityLevel = 31,
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].DragonReliabilityLevel");
    }

    [Fact]
    public void InvalidPartyData_DuplicateCrests_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        CharaData = new() { CharaId = Charas.GalaGatov },
                        CrestSlotType1CrestList = new List<GameAbilityCrest>()
                        {
                            new() { AbilityCrestId = AbilityCrests.CastleCheerCorps },
                        },
                        CrestSlotType2CrestList = new List<GameAbilityCrest>()
                        {
                            new() { AbilityCrestId = AbilityCrests.CastleCheerCorps }
                        }
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0]")
            .WithErrorMessage("Duplicate ability crests");
    }

    [Fact]
    public void InvalidPartyData_InvalidCharaId_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { CharaData = new() { CharaId = (Charas)1234, }, }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].CharaData.CharaId");
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0].CharaData")
            .WithErrorMessage("Element lock violation");
    }

    [Fact]
    public void InvalidPartyData_InvalidCharaAugmentCount_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        CharaData = new()
                        {
                            CharaId = Charas.GalaGatov,
                            AttackPlusCount = 101,
                            HpPlusCount = 101
                        },
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].CharaData.AttackPlusCount");
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].CharaData.HpPlusCount");
    }

    [Fact]
    public void InvalidPartyData_CharaElementLockViolation_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { CharaData = new() { CharaId = Charas.SummerIeyasu }, }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0].CharaData")
            .WithErrorMessage("Element lock violation");
    }

    [Fact]
    public void InvalidPartyData_InvalidDragonId_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { DragonData = new() { DragonId = (Dragons)1234, }, }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].DragonData.DragonId");
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0].DragonData")
            .WithErrorMessage("Element lock violation");
    }

    [Fact]
    public void InvalidPartyData_InvalidDragonAugmentCount_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        DragonData = new()
                        {
                            DragonId = Dragons.GalaRebornAgni,
                            AttackPlusCount = 101,
                            HpPlusCount = 101
                        },
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].DragonData.AttackPlusCount");
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].DragonData.HpPlusCount");
    }

    [Fact]
    public void InvalidPartyData_DragonElementLockViolation_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { DragonData = new() { DragonId = Dragons.SummerMarishiten }, }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0].DragonData")
            .WithErrorMessage("Element lock violation");
    }

    [Fact]
    public void InvalidPartyData_InvalidCrestId_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        CrestSlotType1CrestList = new List<GameAbilityCrest>()
                        {
                            new() { AbilityCrestId = (AbilityCrests)1234 }
                        },
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0][0].AbilityCrestId"); // wtf is this property name?
    }

    [Fact]
    public void InvalidPartyData_InvalidCrestAugments_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        CrestSlotType1CrestList = new List<GameAbilityCrest>()
                        {
                            new() { AttackPlusCount = 101, HpPlusCount = 101 }
                        },
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0][0].AttackPlusCount");
        result.ShouldHaveValidationErrorFor("PartyUnitList[0][0].HpPlusCount");
    }

    [Fact]
    public void InvalidPartyData_InvalidWeaponId_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { WeaponBodyData = new() { WeaponBodyId = (WeaponBodies)1234 } }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].WeaponBodyData.WeaponBodyId");
    }

    [Fact]
    public void InvalidPartyData_InvalidTalismanStats_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        TalismanData = new()
                        {
                            TalismanId = Talismans.GalaLuca,
                            AdditionalAttack = 101,
                            AdditionalHp = 101
                        }
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData.AdditionalAttack");
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData.AdditionalHp");
    }

    [Fact]
    public void InvalidPartyData_InvalidTalismanId_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new() { TalismanData = new() { TalismanId = (Talismans)1234, } }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData.TalismanId");
    }

    [Fact]
    public void InvalidPartyData_InvalidTalismanAbility_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        TalismanData = new()
                        {
                            TalismanId = Talismans.GalaLuca,
                            TalismanAbilityId1 = 330000569,
                            TalismanAbilityId2 = 3004
                        }
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(2);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData.TalismanAbilityId1");
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData.TalismanAbilityId2");
    }

    [Fact]
    public void InvalidPartyData_DuplicateTalismanAbility_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        TalismanData = new()
                        {
                            TalismanId = Talismans.GalaLuca,
                            TalismanAbilityId1 = 340000029,
                            TalismanAbilityId2 = 340000029
                        }
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result
            .ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData")
            .WithErrorMessage("Duplicate talisman abilities");
    }

    [Fact]
    public void InvalidPartyData_ThirdTalismanAbility_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                PartyUnitList = new List<PartyUnitList>()
                {
                    new()
                    {
                        TalismanData = new()
                        {
                            TalismanId = Talismans.GalaLuca,
                            TalismanAbilityId1 = 340000029,
                            TalismanAbilityId2 = 340000077,
                            TalismanAbilityId3 = 340000118,
                        }
                    }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor("PartyUnitList[0].TalismanData.TalismanAbilityId3");
    }
}
