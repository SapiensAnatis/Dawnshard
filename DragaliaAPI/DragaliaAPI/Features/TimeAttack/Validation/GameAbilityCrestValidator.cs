using DragaliaAPI.Models.Generated;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class GameAbilityCrestValidator : AbstractValidator<GameAbilityCrest>
{
    public GameAbilityCrestValidator()
    {
        RuleFor(x => x.AbilityCrestId).IsInEnum();
        RuleFor(x => x.AttackPlusCount).InclusiveBetween(0, 100);
        RuleFor(x => x.HpPlusCount).InclusiveBetween(0, 100);
    }
}
