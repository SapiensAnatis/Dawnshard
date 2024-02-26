using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class PartyInfoValidator : AbstractValidator<PartyInfo>
{
    public PartyInfoValidator(int questId)
    {
        RuleForEach(x => x.PartyUnitList).SetValidator(new PartyUnitListValidator(questId));

        RuleFor(x => x.PartyUnitList)
            .Must(x =>
            {
                IEnumerable<Charas?> ids = x.Select(y => y.CharaData?.CharaId)
                    .Where(y => y != null);

                return ids.Distinct().Count() == ids.Count();
            })
            .WithMessage("Duplicate characters in party");
    }
}
