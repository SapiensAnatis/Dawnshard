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
    public async Task<double> GetFacilityEventMultiplier(
        IEnumerable<PartySettingList> partySettingList,
        int eventId
    )
    {
        double percentTotal = 0;

        IEnumerable<AbilityCrests> equippedCrestIds = partySettingList.SelectMany(
            y => y.GetAbilityCrestList()
        );

        IEnumerable<DbAbilityCrest> equippedCrests = await abilityCrestRepository.AbilityCrests
            .Where(x => equippedCrestIds.Contains(x.AbilityCrestId))
            .ToListAsync();

        foreach (PartySettingList partyUnit in partySettingList)
        {
            IEnumerable<DbAbilityCrest> dbCrests = equippedCrests.IntersectBy(
                partyUnit.GetAbilityCrestList(),
                x => x.AbilityCrestId
            );

            IEnumerable<AbilityData> buildAbilities = this.GetUnitCrestAbilities(dbCrests)
                .Where(x => x.AbilityType1 == AbilityTypes.BuildEventBoost && x.EventId == eventId)
                .ToList();

            logger.LogTrace(
                "Unit {unitNo} build event abilities: {@abilities}",
                partyUnit.unit_no,
                buildAbilities
            );

            if (!buildAbilities.Any())
                continue;

            // After filtering by event ID, they should all have the same AbilityLimitedGroupId1
            Debug.Assert(
                buildAbilities.All(
                    x => x.AbilityLimitedGroupId1 == buildAbilities.First().AbilityLimitedGroupId1
                )
            );

            double abilityLimit = MasterAsset.AbilityLimitedGroup[
                buildAbilities.First().AbilityLimitedGroupId1
            ].MaxLimitedValue;

            double sum = buildAbilities.Sum(x => x.AbilityType1UpValue);

            // Note: other abilities, such as player XP boosts, may be capped per-party instead of per-unit.
            percentTotal += Math.Min(sum, abilityLimit);
        }

        logger.LogDebug(
            "Calculated build event multiplier for event {eventId}: {result}%",
            eventId,
            percentTotal
        );

        return 1 + (percentTotal / 100);
    }

    private IEnumerable<AbilityData> GetUnitCrestAbilities(IEnumerable<DbAbilityCrest> crests)
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
