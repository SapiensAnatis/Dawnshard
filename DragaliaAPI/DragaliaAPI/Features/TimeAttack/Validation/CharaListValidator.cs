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
        RuleFor(x => x.chara_id).IsInEnum();
        RuleFor(x => x.hp).LessThan(1500);
        RuleFor(x => x.attack).LessThan(1500);

        RuleFor(x => x.attack_plus_count).InclusiveBetween(0, 100);
        RuleFor(x => x.hp_plus_count).InclusiveBetween(0, 100);

        if (
            MasterAsset.QuestData.TryGetValue(questId, out QuestData? questData)
            && questData.LimitedElementalType != UnitElement.None
        )
        {
            RuleFor(x => MasterAsset.CharaData.GetValueOrDefault(x.chara_id))
                .Must(x => x?.ElementalType == questData.LimitedElementalType)
                .WithMessage("Element lock violation");
        }
    }
}
