using System.Collections.Frozen;
using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Dungeon;

public static class AbilityLogic
{
    private static readonly FrozenSet<AbilityTypes> RewardAbilityTypes = FrozenSet.Create(
        AbilityTypes.CoinUp,
        AbilityTypes.ManaUp,
        AbilityTypes.BuildEventPointBoost,
        AbilityTypes.DefensiveEventPointBoost,
        AbilityTypes.BuildEventMaterialBoost,
        AbilityTypes.CharaExpUp,
        AbilityTypes.UserExpUp
    );

    public static List<List<int>> GetRewardBoostingAbilitiesByUnit(
        IReadOnlyList<DbDetailedPartyUnit> detailedPartyUnits
    )
    {
        List<List<int>> abilitiesByUnit = [];

        foreach (DbDetailedPartyUnit detailedPartyUnit in detailedPartyUnits)
        {
            List<int> abilities = [];

            if (detailedPartyUnit.CharaData is { } charaData)
            {
                CharaData masterAssetData = MasterAsset.CharaData[charaData.CharaId];

                abilities.Add(
                    masterAssetData.GetAbility(abilityNo: 1, level: charaData.Ability1Level)
                );
                abilities.Add(
                    masterAssetData.GetAbility(abilityNo: 2, level: charaData.Ability2Level)
                );
                abilities.Add(
                    masterAssetData.GetAbility(abilityNo: 3, level: charaData.Ability3Level)
                );
            }

            if (detailedPartyUnit.DragonData is { } dragonData)
            {
                DragonData masterAssetData = MasterAsset.DragonData[dragonData.DragonId];

                abilities.Add(
                    masterAssetData.GetAbility(abilityNo: 1, level: dragonData.Ability1Level)
                );
                abilities.Add(
                    masterAssetData.GetAbility(abilityNo: 2, level: dragonData.Ability2Level)
                );
            }

            IEnumerable<DbAbilityCrest?> crests =
            [
                .. detailedPartyUnit.CrestSlotType1CrestList,
                .. detailedPartyUnit.CrestSlotType2CrestList,
                .. detailedPartyUnit.CrestSlotType3CrestList,
            ];

            foreach (DbAbilityCrest crest in crests.OfType<DbAbilityCrest>())
            {
                AbilityCrest masterAssetData = MasterAsset.AbilityCrest[crest.AbilityCrestId];

                abilities.AddRange(masterAssetData.GetAbilities(crest.AbilityLevel));
            }

            List<int> relevantAbilities = abilities
                .Where(abilityId =>
                    abilityId != 0
                    && RewardAbilityTypes.Contains(MasterAsset.AbilityData[abilityId].AbilityType1)
                )
                .ToList();

            abilitiesByUnit.Add(relevantAbilities);
        }

        return abilitiesByUnit;
    }

    public static double CalculateEventPointMultiplier(
        IEnumerable<IEnumerable<int>> abilityIdsPerUnit,
        int eventId
    )
    {
        return CalculateCappedAbilityMultiplier(
            abilityIdsPerUnit,
            ability =>
                ability.EventId == eventId
                && ability.AbilityType1
                    is AbilityTypes.BuildEventPointBoost
                        or AbilityTypes.DefensiveEventPointBoost
        );
    }

    public static double CalculateEventMaterialMultiplier(
        IEnumerable<IEnumerable<int>> abilityIdsPerUnit,
        int eventId
    )
    {
        return CalculateCappedAbilityMultiplier(
            abilityIdsPerUnit,
            ability =>
                ability.EventId == eventId
                && ability.AbilityType1 == AbilityTypes.BuildEventMaterialBoost
        );
    }

    public static double CalculateCoinMultiplier(IEnumerable<IEnumerable<int>> abilityIdsPerUnit)
    {
        return CalculateAbilityMultiplier(
            abilityIdsPerUnit,
            static ability => ability.AbilityType1 == AbilityTypes.CoinUp
        );
    }

    public static double CalculateManaMultiplier(IEnumerable<IEnumerable<int>> abilityIdsPerUnit)
    {
        return CalculateAbilityMultiplier(
            abilityIdsPerUnit,
            static ability => ability.AbilityType1 == AbilityTypes.ManaUp
        );
    }

    private static double CalculateAbilityMultiplier(
        IEnumerable<IEnumerable<int>> abilityIdsPerUnit,
        Func<AbilityData, bool> filter
    )
    {
        if (abilityIdsPerUnit.TryGetNonEnumeratedCount(out int count) && count == 0)
        {
            return 1;
        }

        double boost = 0;

        foreach (IEnumerable<int> abilityIds in abilityIdsPerUnit)
        {
            boost += abilityIds
                .Select(x => MasterAsset.AbilityData[x])
                .Where(filter)
                .Sum(x => x.AbilityType1UpValue);
        }

        return 1 + (boost / 100);
    }

    private static double CalculateCappedAbilityMultiplier(
        IEnumerable<IEnumerable<int>> abilityIdsPerUnit,
        Func<AbilityData, bool> filter
    )
    {
        if (abilityIdsPerUnit.TryGetNonEnumeratedCount(out int count) && count == 0)
        {
            return 1;
        }

        double boost = 0;

        foreach (IEnumerable<int> abilityIds in abilityIdsPerUnit)
        {
            List<AbilityData> abilityData = abilityIds
                .Select(x => MasterAsset.AbilityData[x])
                .Where(filter)
                .ToList();

            boost += CalculateCappedMultiplierPercentageValue(abilityData);
        }

        return 1 + (boost / 100);
    }

    private static double CalculateCappedMultiplierPercentageValue(List<AbilityData> abilities)
    {
        if (abilities.Count == 0)
        {
            return 0;
        }

        // After filtering by event ID, they should all have the same AbilityLimitedGroupId1
        Debug.Assert(
            abilities.All(x => x.AbilityLimitedGroupId1 == abilities[0].AbilityLimitedGroupId1)
        );

        double abilityLimit = MasterAsset
            .AbilityLimitedGroup[abilities[0].AbilityLimitedGroupId1]
            .MaxLimitedValue;

        double sum = abilities.Sum(x => x.AbilityType1UpValue);

        // Note: other abilities, such as player XP boosts, may be capped per-party instead of per-unit.
        return Math.Min(sum, abilityLimit);
    }
}
