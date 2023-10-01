using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { chara_data = new() { chara_id = Charas.GalaGatov } },
                    new() { chara_data = new() { chara_id = Charas.GalaGatov } }
                }
            }
        );

        result.Errors.Count.Should().Be(1);
        result.ShouldHaveValidationErrorFor(x => x.party_unit_list);
    }

    [Fact]
    public void InvalidPartyData_DuplicateSharedSkills_ReturnsFail()
    {
        TestValidationResult<PartyInfo> result = validator.TestValidate(
            new PartyInfo()
            {
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        chara_data = new() { chara_id = Charas.GalaGatov },
                        edit_skill_1_chara_data = new() { chara_id = Charas.ShaWujing },
                        edit_skill_2_chara_data = new() { chara_id = Charas.ShaWujing }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        chara_data = new() { chara_id = Charas.GalaGatov },
                        dragon_reliability_level = 31,
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        chara_data = new() { chara_id = Charas.GalaGatov },
                        crest_slot_type_1_crest_list = new List<GameAbilityCrest>()
                        {
                            new() { ability_crest_id = AbilityCrests.CastleCheerCorps },
                        },
                        crest_slot_type_2_crest_list = new List<GameAbilityCrest>()
                        {
                            new() { ability_crest_id = AbilityCrests.CastleCheerCorps }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { chara_data = new() { chara_id = (Charas)1234, }, }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        chara_data = new()
                        {
                            chara_id = Charas.GalaGatov,
                            attack_plus_count = 101,
                            hp_plus_count = 101
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { chara_data = new() { chara_id = Charas.SummerIeyasu }, }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { dragon_data = new() { dragon_id = (Dragons)1234, }, }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        dragon_data = new()
                        {
                            dragon_id = Dragons.GalaRebornAgni,
                            attack_plus_count = 101,
                            hp_plus_count = 101
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { dragon_data = new() { dragon_id = Dragons.SummerMarishiten }, }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        crest_slot_type_1_crest_list = new List<GameAbilityCrest>()
                        {
                            new() { ability_crest_id = (AbilityCrests)1234 }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        crest_slot_type_1_crest_list = new List<GameAbilityCrest>()
                        {
                            new() { attack_plus_count = 101, hp_plus_count = 101 }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { weapon_body_data = new() { weapon_body_id = (WeaponBodies)1234 } }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        talisman_data = new()
                        {
                            talisman_id = Talismans.GalaLuca,
                            additional_attack = 101,
                            additional_hp = 101
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new() { talisman_data = new() { talisman_id = (Talismans)1234, } }
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        talisman_data = new()
                        {
                            talisman_id = Talismans.GalaLuca,
                            talisman_ability_id_1 = 330000569,
                            talisman_ability_id_2 = 3004
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        talisman_data = new()
                        {
                            talisman_id = Talismans.GalaLuca,
                            talisman_ability_id_1 = 340000029,
                            talisman_ability_id_2 = 340000029
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
                party_unit_list = new List<PartyUnitList>()
                {
                    new()
                    {
                        talisman_data = new()
                        {
                            talisman_id = Talismans.GalaLuca,
                            talisman_ability_id_1 = 340000029,
                            talisman_ability_id_2 = 340000077,
                            talisman_ability_id_3 = 340000118,
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
