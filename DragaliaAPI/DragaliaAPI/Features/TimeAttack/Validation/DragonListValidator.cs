using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class DragonListValidator : AbstractValidator<DragonList>
{
    public DragonListValidator(int questId)
    {
        RuleFor(x => x.HpPlusCount).InclusiveBetween(0, 100);
        RuleFor(x => x.AttackPlusCount).InclusiveBetween(0, 100);

        RuleFor(x => x.DragonId).IsInEnum();

        if (
            MasterAsset.QuestData.TryGetValue(questId, out QuestData? questData)
            && questData.LimitedElementalType != UnitElement.None
        )
        {
            RuleFor(x => MasterAsset.DragonData.GetValueOrDefault(x.DragonId))
                .Must(x => x?.ElementalType == questData.LimitedElementalType)
                .WithMessage("Element lock violation");
        }
    }
}
