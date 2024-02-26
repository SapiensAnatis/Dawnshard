using DragaliaAPI.Models.Generated;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class TalismanValidator : AbstractValidator<TalismanList>
{
    public TalismanValidator()
    {
        RuleFor(x => x.AdditionalAttack).InclusiveBetween(0, 100);
        RuleFor(x => x.AdditionalHp).InclusiveBetween(0, 100);

        RuleFor(x => x.TalismanId).IsInEnum();

        When(
            x => x.TalismanAbilityId1 != 0,
            () => RuleFor(x => x.TalismanAbilityId1).ExclusiveBetween(340000000, 350000000)
        );

        When(
            x => x.TalismanAbilityId2 != 0,
            () => RuleFor(x => x.TalismanAbilityId2).ExclusiveBetween(340000000, 350000000)
        );

        When(
            x => x.TalismanAbilityId1 != 0 && x.TalismanAbilityId2 != 0,
            () =>
                RuleFor(x => new
                    {
                        talisman_ability_id_1 = x.TalismanAbilityId1,
                        talisman_ability_id_2 = x.TalismanAbilityId2
                    })
                    .Must(x => x.talisman_ability_id_1 != x.talisman_ability_id_2)
                    .WithMessage("Duplicate talisman abilities")
        );

        RuleFor(x => x.TalismanAbilityId3).Equal(0);
    }
}
