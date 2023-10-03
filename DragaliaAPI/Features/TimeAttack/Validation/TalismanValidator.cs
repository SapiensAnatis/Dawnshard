using DragaliaAPI.Models.Generated;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class TalismanValidator : AbstractValidator<TalismanList>
{
    public TalismanValidator()
    {
        RuleFor(x => x.additional_attack).InclusiveBetween(0, 100);
        RuleFor(x => x.additional_hp).InclusiveBetween(0, 100);

        RuleFor(x => x.talisman_id).IsInEnum();

        When(
            x => x.talisman_ability_id_1 != 0,
            () => RuleFor(x => x.talisman_ability_id_1).ExclusiveBetween(340000000, 350000000)
        );

        When(
            x => x.talisman_ability_id_2 != 0,
            () => RuleFor(x => x.talisman_ability_id_2).ExclusiveBetween(340000000, 350000000)
        );

        When(
            x => x.talisman_ability_id_1 != 0 && x.talisman_ability_id_2 != 0,
            () =>
                RuleFor(x => new { x.talisman_ability_id_1, x.talisman_ability_id_2 })
                    .Must(x => x.talisman_ability_id_1 != x.talisman_ability_id_2)
                    .WithMessage("Duplicate talisman abilities")
        );

        RuleFor(x => x.talisman_ability_id_3).Equal(0);
    }
}
