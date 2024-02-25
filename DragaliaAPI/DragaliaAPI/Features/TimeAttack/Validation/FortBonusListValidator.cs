using DragaliaAPI.Models.Generated;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class FortBonusListValidator : AbstractValidator<FortBonusList>
{
    public FortBonusListValidator()
    {
        RuleForEach(x => x.ParamBonusByWeapon)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(30);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(30);
            });

        RuleForEach(x => x.ParamBonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(45);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(45);
            });

        RuleForEach(x => x.ElementBonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(80);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(80);
            });

        RuleForEach(x => x.DragonBonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.DragonBonus).LessThanOrEqualTo(50);
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(11.5f);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(11.5f);
            });

        RuleForEach(x => x.DragonBonusByAlbum)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(8);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(8);
            });

        RuleForEach(x => x.CharaBonusByAlbum)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(15);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(15);
            });

        RuleFor(x => x.DragonTimeBonus)
            .ChildRules(x => x.RuleFor(y => y.DragonTimeBonus).LessThanOrEqualTo(0));

        RuleFor(x => x.AllBonus)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.Hp).LessThanOrEqualTo(0);
                x.RuleFor(y => y.Attack).LessThanOrEqualTo(0);
            });
    }
}
