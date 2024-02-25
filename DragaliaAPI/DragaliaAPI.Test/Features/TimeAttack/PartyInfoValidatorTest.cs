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
            .ShouldHaveValidationErrorFor("party_unit_list[0]")
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].dragon_reliability_level");
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
            .ShouldHaveValidationErrorFor("party_unit_list[0]")
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].chara_data.chara_id");
        result
            .ShouldHaveValidationErrorFor("party_unit_list[0].chara_data")
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].chara_data.attack_plus_count");
        result.ShouldHaveValidationErrorFor("party_unit_list[0].chara_data.hp_plus_count");
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
            .ShouldHaveValidationErrorFor("party_unit_list[0].chara_data")
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].dragon_data.dragon_id");
        result
            .ShouldHaveValidationErrorFor("party_unit_list[0].dragon_data")
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].dragon_data.attack_plus_count");
        result.ShouldHaveValidationErrorFor("party_unit_list[0].dragon_data.hp_plus_count");
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
            .ShouldHaveValidationErrorFor("party_unit_list[0].dragon_data")
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0][0].ability_crest_id"); // wtf is this property name?
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0][0].attack_plus_count");
        result.ShouldHaveValidationErrorFor("party_unit_list[0][0].hp_plus_count");
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].weapon_body_data.weapon_body_id");
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].talisman_data.additional_attack");
        result.ShouldHaveValidationErrorFor("party_unit_list[0].talisman_data.additional_hp");
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
        result.ShouldHaveValidationErrorFor("party_unit_list[0].talisman_data.talisman_id");
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
        result.ShouldHaveValidationErrorFor(
            "party_unit_list[0].talisman_data.talisman_ability_id_1"
        );
        result.ShouldHaveValidationErrorFor(
            "party_unit_list[0].talisman_data.talisman_ability_id_2"
        );
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
            .ShouldHaveValidationErrorFor("party_unit_list[0].talisman_data")
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
        result.ShouldHaveValidationErrorFor(
            "party_unit_list[0].talisman_data.talisman_ability_id_3"
        );
    }
}
