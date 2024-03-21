using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentRandomPicker;
using FluentRandomPicker.FluentInterfaces.General;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Summoning;

public class SummonService(
    SummonOddsService summonOddsService,
    IUnitRepository unitRepository,
    ApiContext apiContext
) : ISummonService
{
    /// <summary>
    /// The factor to use when converting the <see cref="decimal"/> rate to a <see cref="int"/> weighting for
    /// FluentRandomPicker.
    /// </summary>
    /// <remarks>
    /// A value of 100_000 means that a rate of 41.125% / 0.41125 does not lose any precision and is converted to
    /// 41125. 3 decimal places of precision on the percentage is most likely 'good enough'.
    /// </remarks>
    private const int RateConversionFactor = 100_000;

    public Task<List<AtgenRedoableSummonResultUnitList>> GenerateSummonResult(
        int numSummons,
        int bannerId,
        SummonExecTypes execType
    ) =>
        execType switch
        {
            SummonExecTypes.Single
            or SummonExecTypes.DailyDeal
                => this.GenerateSummonResultInternal(bannerId, numSummons),
            SummonExecTypes.Tenfold => this.GenerateTenfoldResultInternal(bannerId, numTenfolds: 1),
            _ => throw new ArgumentException($"Invalid summon type {execType}", nameof(execType)),
        };

    public Task<List<AtgenRedoableSummonResultUnitList>> GenerateRedoableSummonResult() =>
        this.GenerateTenfoldResultInternal(SummonConstants.RedoableSummonBannerId, numTenfolds: 5);

    /// <summary>
    /// Populate a summon result with is_new and eldwater values.
    /// </summary>
    public List<AtgenResultUnitList> GenerateRewardList(
        IEnumerable<AtgenRedoableSummonResultUnitList> baseRewardList
    )
    {
        List<AtgenResultUnitList> newUnits = new();

        IEnumerable<Charas> ownedCharas = unitRepository.Charas.Select(x => x.CharaId);

        IEnumerable<Dragons> ownedDragons = unitRepository.Dragons.Select(x => x.DragonId);

        foreach (AtgenRedoableSummonResultUnitList reward in baseRewardList)
        {
            bool isNew = newUnits.All(x => x.Id != reward.Id);

            switch (reward.EntityType)
            {
                case EntityTypes.Chara:
                {
                    isNew |= ownedCharas.All(x => x != (Charas)reward.Id);

                    AtgenResultUnitList toAdd =
                        new(
                            reward.EntityType,
                            reward.Id,
                            reward.Rarity,
                            isNew,
                            3,
                            isNew ? 0 : DewValueData.DupeSummon[reward.Rarity]
                        );

                    newUnits.Add(toAdd);
                    break;
                }
                case EntityTypes.Dragon:
                {
                    isNew |= ownedDragons.All(x => x != (Dragons)reward.Id);

                    AtgenResultUnitList toAdd =
                        new(reward.EntityType, reward.Id, reward.Rarity, isNew, 3, 0);

                    newUnits.Add(toAdd);
                    break;
                }
                default:
                    throw new UnreachableException(
                        "Invalid entity type for redoable summon result."
                    );
            }
        }

        return newUnits;
    }

    public async Task<IEnumerable<SummonTicketList>> GetSummonTicketList() =>
        await apiContext.PlayerSummonTickets.ProjectToSummonTicketList().ToListAsync();

    private async Task<List<AtgenRedoableSummonResultUnitList>> GenerateSummonResultInternal(
        int bannerId,
        int numSummons
    )
    {
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) rateData =
            await summonOddsService.GetUnitRates(bannerId);

        List<UnitRate> allRates = [.. rateData.PickupRates, .. rateData.NormalRates];

        IPick<AtgenRedoableSummonResultUnitList> picker = Out.Of()
            .PrioritizedElements(allRates)
            .WithValueSelector(ToSummonResult)
            .AndWeightSelector(x => (int)(x.Rate * RateConversionFactor));

        List<AtgenRedoableSummonResultUnitList> result = new(numSummons);

        for (int i = 0; i < numSummons; i++)
            result.Add(picker.PickOne());

        return result;
    }

    private async Task<List<AtgenRedoableSummonResultUnitList>> GenerateTenfoldResultInternal(
        int bannerId,
        int numTenfolds
    )
    {
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) normalRateData =
            await summonOddsService.GetUnitRates(bannerId);
        (IEnumerable<UnitRate> PickupRates, IEnumerable<UnitRate> NormalRates) guaranteeRateData =
            await summonOddsService.GetGuaranteeUnitRates(bannerId);

        List<UnitRate> allNormalRates =
        [
            .. normalRateData.PickupRates,
            .. normalRateData.NormalRates
        ];
        List<UnitRate> allGuaranteeRates =
        [
            .. guaranteeRateData.PickupRates,
            .. guaranteeRateData.NormalRates
        ];

        IPick<AtgenRedoableSummonResultUnitList> normalPicker = Out.Of()
            .PrioritizedElements(allNormalRates)
            .WithValueSelector(ToSummonResult)
            .AndWeightSelector(x => x.Weighting);

        IPick<AtgenRedoableSummonResultUnitList> guaranteePicker = Out.Of()
            .PrioritizedElements(allGuaranteeRates)
            .WithValueSelector(ToSummonResult)
            .AndWeightSelector(x => x.Weighting);

        List<AtgenRedoableSummonResultUnitList> result = new(10);

        for (int tenfold = 0; tenfold < numTenfolds; tenfold++)
        {
            for (int i = 0; i < 9; i++)
                result.Add(normalPicker.PickOne());

            result.Add(guaranteePicker.PickOne());
        }

        return result;
    }

    private static AtgenRedoableSummonResultUnitList ToSummonResult(UnitRate rate) =>
        new()
        {
            Id = rate.Id,
            EntityType = rate.EntityType,
            Rarity = rate.Rarity,
        };
}
