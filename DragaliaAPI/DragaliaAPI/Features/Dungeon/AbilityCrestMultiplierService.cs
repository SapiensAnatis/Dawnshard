using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon;

public class AbilityCrestMultiplierService(
    IAbilityCrestRepository abilityCrestRepository,
    ILogger<AbilityCrestMultiplierService> logger
) : IAbilityCrestMultiplierService
{
    public async Task<(double Material, double Point)> GetEventMultiplier(
        IEnumerable<PartySettingList> partySettingList,
        int eventId
    )
    {
        double materialBoost = 0;
        double pointBoost = 0;

        IEnumerable<AbilityCrests> equippedCrestIds = partySettingList.SelectMany(y =>
            y.GetAbilityCrestList()
        );

        IEnumerable<DbAbilityCrest> equippedCrests = await abilityCrestRepository
            .AbilityCrests.Where(x => equippedCrestIds.Contains(x.AbilityCrestId))
            .ToListAsync();

        foreach (PartySettingList partyUnit in partySettingList)
        {
            IEnumerable<DbAbilityCrest> dbCrests = equippedCrests
                .IntersectBy(partyUnit.GetAbilityCrestList(), x => x.AbilityCrestId)
                .ToList();

            List<AbilityData> buildPointAbilities = GetUnitCrestAbilities(dbCrests)
                .Where(x =>
                    x.AbilityType1
                        is AbilityTypes.BuildEventPointBoost
                            or AbilityTypes.DefensiveEventPointBoost
                    && x.EventId == eventId
                )
                .ToList();

            logger.LogTrace(
                "Unit {unitNo} build event point abilities: {@abilities}",
                partyUnit.UnitNo,
                buildPointAbilities
            );

            pointBoost += CalculatePercentageValue(buildPointAbilities);

            List<AbilityData> buildMaterialAbilities = GetUnitCrestAbilities(dbCrests)
                .Where(x =>
                    x.AbilityType1 is AbilityTypes.BuildEventMaterialBoost && x.EventId == eventId
                )
                .ToList();

            logger.LogTrace(
                "Unit {unitNo} build event material abilities: {@abilities}",
                partyUnit.UnitNo,
                buildPointAbilities
            );

            materialBoost += CalculatePercentageValue(buildMaterialAbilities);
        }

        logger.LogDebug(
            "Calculated build event multipliers for event {eventId}: Material {materialMultiplier}% / Points {pointMultiplier}%",
            eventId,
            materialBoost,
            pointBoost
        );

        return (1 + (materialBoost / 100), 1 + (pointBoost / 100));

        double CalculatePercentageValue(IReadOnlyCollection<AbilityData> abilities)
        {
            if (abilities.Count == 0)
                return 0;

            // After filtering by event ID, they should all have the same AbilityLimitedGroupId1
            Debug.Assert(
                abilities.All(x =>
                    x.AbilityLimitedGroupId1 == abilities.First().AbilityLimitedGroupId1
                )
            );

            double abilityLimit = MasterAsset
                .AbilityLimitedGroup[abilities.First().AbilityLimitedGroupId1]
                .MaxLimitedValue;

            double sum = abilities.Sum(x => x.AbilityType1UpValue);

            // Note: other abilities, such as player XP boosts, may be capped per-party instead of per-unit.
            return Math.Min(sum, abilityLimit);
        }
    }

    private static List<AbilityData> GetUnitCrestAbilities(IEnumerable<DbAbilityCrest> crests)
    {
        List<AbilityData> result = new();

        foreach (DbAbilityCrest crest in crests)
        {
            if (
                !MasterAsset.AbilityCrest.TryGetValue(
                    crest.AbilityCrestId,
                    out AbilityCrest? crestData
                )
            )
            {
                continue;
            }

            // It seems that both XP boost and facility boost abilities take up slot 1 and not any of the other slots.
            result.AddRange(
                crestData
                    .GetAbilities(crest.AbilityLevel)
                    .Where(abilityId => abilityId != 0)
                    .Select(abilityId => MasterAsset.AbilityData[abilityId])
            );
        }

        return result;
    }
}
