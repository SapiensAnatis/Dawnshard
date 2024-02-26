using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class CharaListValidator : AbstractValidator<CharaList>
{
    public CharaListValidator(int questId)
    {
        RuleFor(x => x.CharaId).IsInEnum();
        RuleFor(x => x.Hp).LessThan(1500);
        RuleFor(x => x.Attack).LessThan(1500);

        RuleFor(x => x.AttackPlusCount).InclusiveBetween(0, 100);
        RuleFor(x => x.HpPlusCount).InclusiveBetween(0, 100);

        if (
            MasterAsset.QuestData.TryGetValue(questId, out QuestData? questData)
            && questData.LimitedElementalType != UnitElement.None
        )
        {
            RuleFor(x => MasterAsset.CharaData.GetValueOrDefault(x.CharaId))
                .Must(x => x?.ElementalType == questData.LimitedElementalType)
                .WithMessage("Element lock violation");
        }
    }
}
