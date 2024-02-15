using DragaliaAPI.Models.Generated;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class FortBonusListValidator : AbstractValidator<FortBonusList>
{
    public FortBonusListValidator()
    {
        RuleForEach(x => x.param_bonus_by_weapon)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.hp).LessThanOrEqualTo(30);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(30);
            });

        RuleForEach(x => x.param_bonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.hp).LessThanOrEqualTo(45);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(45);
            });

        RuleForEach(x => x.element_bonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.hp).LessThanOrEqualTo(80);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(80);
            });

        RuleForEach(x => x.dragon_bonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.dragon_bonus).LessThanOrEqualTo(50);
                x.RuleFor(y => y.hp).LessThanOrEqualTo(11.5f);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(11.5f);
            });

        RuleForEach(x => x.dragon_bonus_by_album)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.hp).LessThanOrEqualTo(8);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(8);
            });

        RuleForEach(x => x.chara_bonus_by_album)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.hp).LessThanOrEqualTo(15);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(15);
            });

        RuleFor(x => x.dragon_time_bonus)
            .ChildRules(x => x.RuleFor(y => y.dragon_time_bonus).LessThanOrEqualTo(0));

        RuleFor(x => x.all_bonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.hp).LessThanOrEqualTo(0);
                x.RuleFor(y => y.attack).LessThanOrEqualTo(0);
            });
    }
}
