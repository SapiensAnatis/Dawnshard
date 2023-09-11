using System.Security.Permissions;
using DragaliaAPI.Models.Generated;
using FluentValidation;
using FluentValidation.Validators;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class GameAbilityCrestValidator : AbstractValidator<GameAbilityCrest>
{
    public GameAbilityCrestValidator()
    {
        RuleFor(x => x.ability_crest_id).IsInEnum();
        RuleFor(x => x.attack_plus_count).InclusiveBetween(0, 100);
        RuleFor(x => x.hp_plus_count).InclusiveBetween(0, 100);
    }
}
