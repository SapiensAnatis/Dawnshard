using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class PartyUnitListValidator : AbstractValidator<PartyUnitList>
{
    public PartyUnitListValidator(int questId)
    {
        When(
            x =>
                x.edit_skill_1_chara_data is not null and not { chara_id: Charas.Empty }
                && x.edit_skill_2_chara_data is not null and not { chara_id: Charas.Empty },
            () =>
                RuleFor(x => new { x.edit_skill_1_chara_data, x.edit_skill_2_chara_data })
                    .Must(
                        x =>
                            x.edit_skill_1_chara_data!.chara_id
                            != x.edit_skill_2_chara_data!.chara_id
                    )
                    .WithMessage("Duplicate shared skills")
        );

        RuleFor(x => x.dragon_reliability_level).LessThanOrEqualTo(30);

        When(
            x => x.chara_data is not null && x.chara_data.chara_id != Charas.Empty,
            () => RuleFor(x => x.chara_data!).SetValidator(new CharaListValidator(questId))
        );

        When(
            x => x.dragon_data is not null && x.dragon_data.dragon_id != Dragons.Empty,
            () => RuleFor(x => x.dragon_data!).SetValidator(new DragonListValidator(questId))
        );

        RuleForEach(x => AllAbilityCrests(x)).SetValidator(new GameAbilityCrestValidator());

        RuleFor(x => AllAbilityCrests(x))
            .Must(x => x.DistinctBy(y => y.ability_crest_id).Count() == x.Count())
            .WithMessage("Duplicate ability crests");

        When(
            x =>
                x.weapon_body_data is not null
                && x.weapon_body_data.weapon_body_id != WeaponBodies.Empty,
            () => RuleFor(x => x.weapon_body_data!).SetValidator(new GameWeaponBodyValidator())
        );

        When(
            x => x.talisman_data is not null && x.talisman_data.talisman_id != Talismans.Empty,
            () => RuleFor(x => x.talisman_data!).SetValidator(new TalismanValidator())
        );
    }

    private static IEnumerable<GameAbilityCrest> AllAbilityCrests(PartyUnitList unit) =>
        unit.crest_slot_type_1_crest_list
            .Concat(unit.crest_slot_type_2_crest_list)
            .Concat(unit.crest_slot_type_3_crest_list);
}
