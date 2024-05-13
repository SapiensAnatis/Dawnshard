using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.DmodeDungeon;

public static class TalismanHelper
{
    private static readonly (int StartId, int Amount)[] Ability1Pool =
    {
        (0, 0), // No ability
        (1, 7), // Strength
        (8, 2), // Element-specific strength
        (10, 1), // Element + Weapon specific strength
        (11, 7), // Skill Damage
        (18, 2), // Element-specific skill damage
        (20, 1), // Element + Weapon specific skill damage
        (21, 7), // Critical Rate
        (28, 2), // Element-specific critical rate
        (30, 1), // Element + Weapon specific critical rate
        (41, 7), // Force Strike
        (48, 2), // Element-specific force strike
        (50, 1), // Element + Weapon specific force strike
        (61, 7), // HP
        (68, 2), // Element-specific hp
        (70, 1), // Element + Weapon specific hp
        (91, 7), // Dragon Damage
        (98, 2), // Element-specific dragon damage
        (100, 1), // Element + Weapon specific dragon damage
        (111, 7), // Dragon Haste
        (118, 2), // Element-specific dragon haste
        (120, 1) // Element + Weapon specific dragon haste
    };

    private static readonly (int StartId, int Amount)[] Ability2Pool =
    {
        (0, 0), // No ability
        (31, 4), // Skill Haste (actually 7 but game only uses 4)
        (38, 2), // Element-specific skill haste
        (40, 1), // Element + Weapon specific skill haste
        (51, 7), // Skill Prep
        (58, 2), // Element-specific skill prep
        (60, 1), // Element + Weapon specific skill prep
        (71, 4), // Defense (actually 7 but game only uses 4)
        (78, 2), // Element-specific defense
        (80, 1), // Element + Weapon specific defense
        (81, 4), // Critical Damage (actually 7 but game only uses 4)
        (88, 2), // Element-specific critical damage
        (90, 1), // Element + Weapon specific critical damage
        (101, 7), // Recovery Potency
        (108, 2), // Element-specific recovery potency
        (110, 1), // Element + Weapon specific recovery potency
        (121, 7), // Dragon Time
        (128, 2), // Element-specific dragon time
        (130, 1), // Element + Weapon specific dragon time
        (131, 4), // Tradeoffs (Steady|Easy|Lucky|Hasty) Hitter I
        (131, 4),
        (131, 4)
    };

    private const int TalismanIdDifference = 40_000_000;
    private const int AdditionalRemoveNum = 50;
    private const int AbilityIdDifference = 340_000_000;

    private static readonly int[] Ability1FloorThresholds =
    {
        10, // For guaranteed ability
        30, // For guaranteed elem-locked
        50, // For guaranteed weapon-locked
    };

    private static readonly int[] Ability2FloorThresholds =
    {
        20, // For guaranteed ability
        40, // For guaranteed elem-locked
        60, // For guaranteed weapon-locked
    };

    public static AtgenRewardTalismanList GenerateTalisman(Random rdm, Charas charaId, int floor)
    {
        Talismans talismanId = (Talismans)((int)charaId + TalismanIdDifference);

        int additionalAtk = 100;
        int additionalHp = 100;

        int abilityId1 = GenerateAbility(rdm, Ability1Pool, floor, Ability1FloorThresholds);
        if (abilityId1 != 0)
        {
            additionalHp -= AdditionalRemoveNum;
            additionalAtk -= AdditionalRemoveNum;
        }

        int abilityId2 = GenerateAbility(rdm, Ability2Pool, floor, Ability2FloorThresholds);
        if (abilityId2 != 0)
        {
            additionalHp -= AdditionalRemoveNum;
            additionalAtk -= AdditionalRemoveNum;
        }

        return new AtgenRewardTalismanList(
            talismanId,
            abilityId1,
            abilityId2,
            0,
            additionalHp,
            additionalAtk
        );
    }

    private static int GenerateAbility(
        Random rdm,
        IReadOnlyList<(int StartId, int Amount)> pool,
        int floor,
        IReadOnlyList<int> thresholds
    )
    {
        int startIndex = floor >= thresholds[0] ? 1 : 0; // Guarantee ability
        int index = rdm.Next(startIndex, pool.Count);
        if (index == 0)
            return 0;

        if (floor == 0)
        {
            // Portrait wyrmprints rewarded from expeditions cannot have any elemental or weapon type restrictions. (wiki)
            // Guarantee non-elem-locked
            while ((index - 1) % 3 != 0)
                index--;
        }
        else if (pool.Count - 1 != index)
        {
            if ((index - 1) % 3 == 0 && floor >= thresholds[1]) // Guarantee elem-locked
                index++;

            if ((index - 1) % 3 == 1 && floor >= thresholds[2]) // Guarantee weapon-locked
                index++;
        }

        (int startId, int amount) = pool[index];

        switch (amount)
        {
            case 0:
                return 0;
            case 1:
                return startId + AbilityIdDifference;
            default:
                Span<int> abilityPool = stackalloc int[amount];
                for (int i = 0; i < amount; i++)
                    abilityPool[i] = i + startId;

                return rdm.Next(abilityPool) + AbilityIdDifference;
        }
    }
}
