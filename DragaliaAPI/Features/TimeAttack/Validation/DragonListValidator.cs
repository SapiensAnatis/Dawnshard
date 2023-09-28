using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class DragonListValidator : AbstractValidator<DragonList>
{
    public DragonListValidator(int questId)
    {
        RuleFor(x => x.hp_plus_count).InclusiveBetween(0, 100);
        RuleFor(x => x.attack_plus_count).InclusiveBetween(0, 100);

        RuleFor(x => x.dragon_id).IsInEnum();

        if (
            MasterAsset.QuestData.TryGetValue(questId, out QuestData? questData)
            && questData.LimitedElementalType != UnitElement.None
        )
        {
            RuleFor(x => MasterAsset.DragonData.GetValueOrDefault(x.dragon_id))
                .Must(x => x?.ElementalType == questData.LimitedElementalType)
                .WithMessage("Element lock violation");
        }
    }
}
