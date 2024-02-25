using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentValidation;

namespace DragaliaAPI.Features.TimeAttack.Validation;

public class PartyUnitListValidator : AbstractValidator<PartyUnitList>
{
    public PartyUnitListValidator(int questId)
    {
        When(
            x =>
                x.EditSkill1CharaData is not null and not { CharaId: Charas.Empty }
                && x.EditSkill2CharaData is not null and not { CharaId: Charas.Empty },
            () =>
                RuleFor(x => new
                    {
                        edit_skill_1_chara_data = x.EditSkill1CharaData,
                        edit_skill_2_chara_data = x.EditSkill2CharaData
                    })
                    .Must(x =>
                        x.edit_skill_1_chara_data!.CharaId != x.edit_skill_2_chara_data!.CharaId
                    )
                    .WithMessage("Duplicate shared skills")
        );

        RuleFor(x => x.DragonReliabilityLevel).LessThanOrEqualTo(30);

        When(
            x => x.CharaData is not null && x.CharaData.CharaId != Charas.Empty,
            () => RuleFor(x => x.CharaData!).SetValidator(new CharaListValidator(questId))
        );

        When(
            x => x.DragonData is not null && x.DragonData.DragonId != Dragons.Empty,
            () => RuleFor(x => x.DragonData!).SetValidator(new DragonListValidator(questId))
        );

        RuleForEach(x => AllAbilityCrests(x)).SetValidator(new GameAbilityCrestValidator());

        RuleFor(x => AllAbilityCrests(x))
            .Must(x => x.DistinctBy(y => y.AbilityCrestId).Count() == x.Count())
            .WithMessage("Duplicate ability crests");

        When(
            x =>
                x.WeaponBodyData is not null && x.WeaponBodyData.WeaponBodyId != WeaponBodies.Empty,
            () => RuleFor(x => x.WeaponBodyData!).SetValidator(new GameWeaponBodyValidator())
        );

        When(
            x => x.TalismanData is not null && x.TalismanData.TalismanId != Talismans.Empty,
            () => RuleFor(x => x.TalismanData!).SetValidator(new TalismanValidator())
        );
    }

    private static IEnumerable<GameAbilityCrest> AllAbilityCrests(PartyUnitList unit) =>
        unit
            .CrestSlotType1CrestList.Concat(unit.CrestSlotType2CrestList)
            .Concat(unit.CrestSlotType3CrestList);
}
