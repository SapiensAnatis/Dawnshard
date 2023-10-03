using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class PartyInfoValidator : AbstractValidator<PartyInfo>
{
    public PartyInfoValidator(int questId)
    {
        RuleForEach(x => x.party_unit_list).SetValidator(new PartyUnitListValidator(questId));

        RuleFor(x => x.party_unit_list)
            .Must(x =>
            {
                IEnumerable<Charas?> ids = x.Select(y => y.chara_data?.chara_id)
                    .Where(y => y != null);

                return ids.Distinct().Count() == ids.Count();
            })
            .WithMessage("Duplicate characters in party");
    }
}
