using DragaliaAPI.Models.Generated;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class GameWeaponBodyValidator : AbstractValidator<GameWeaponBody>
{
    public GameWeaponBodyValidator()
    {
        // We don't care about ability and skill levels etc -- increasing those will just be considered invalid by the client
        RuleFor(x => x.WeaponBodyId).IsInEnum();
    }
}
