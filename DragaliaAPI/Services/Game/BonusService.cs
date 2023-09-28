//#define CHEATS
#if CHEATS && DEBUG
#define CHEATING
#endif

using System.Collections.Immutable;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DragaliaAPI.Services.Game;

/// <summary>
/// Service for fort, weapon, and album bonuses
/// </summary>
public class BonusService(
    IFortRepository fortRepository,
    IWeaponRepository weaponRepository,
    ILogger<BonusService> logger
) : IBonusService
{
    public async Task<FortBonusList> GetBonusList()
    {
        IEnumerable<int> buildIds = (
            await fortRepository.Builds
                .Where(x => x.Level != 0)
                .Select(x => new { x.PlantId, x.Level })
                .ToListAsync()
        ).Select(x => MasterAssetUtils.GetPlantDetailId(x.PlantId, x.Level));

        IEnumerable<WeaponBodies> weaponIds = await weaponRepository.WeaponBodies
            .Where(x => x.FortPassiveCharaWeaponBuildupCount != 0)
            .Select(x => x.WeaponBodyId)
            .ToListAsync();

        return new()
        {
            param_bonus = GetFortParamBonus(buildIds),
            element_bonus = GetFortElementBonus(buildIds),
            dragon_bonus = GetFortDragonBonus(buildIds),
            param_bonus_by_weapon = GetWeaponParamBonus(weaponIds),
            // TODO: Implement these bonuses
            chara_bonus_by_album = StubData.MaxAlbumCharaBonus,
            dragon_bonus_by_album = StubData.MaxAlbumDragonBonus,
            // These are all 0 on my endgame save. Unsure of what, if anything, may increase them
            dragon_time_bonus = new()
            {
#if CHEATING
                dragon_time_bonus = 20
#endif
            },
            all_bonus = new()
            {
#if CHEATING
                attack = 100,
                hp = 100
#endif
            },
        };
    }

    public async Task<AtgenEventBoost?> GetEventBoost(int eventId)
    {
        if (
            !MasterAsset.EventData.TryGetValue(eventId, out EventData? eventData)
            || eventData.EventFortId == 0
        )
        {
            logger.LogDebug("No event facility found for eventId {eventId}", eventId);
            return null;
        }

        int level = await fortRepository.Builds
            .Where(x => x.PlantId == eventData.EventFortId)
            .Select(x => x.Level)
            .SingleOrDefaultAsync();

        if (level == 0)
        {
            logger.LogDebug("Player did not own event facility {facility}", eventData.EventFortId);
            return null;
        }

        FortPlantDetail detail = MasterAssetUtils.GetFortPlant(eventData.EventFortId, level);

        return new AtgenEventBoost()
        {
            event_effect = detail.EventEffectType,
            effect_value = detail.EventEffectArgs
        };
    }

    private static IEnumerable<AtgenElementBonus> GetFortElementBonus(IEnumerable<int> buildIds)
    {
        IDictionary<UnitElement, AtgenElementBonus> result = Enum.GetValues<UnitElement>()
            .Select(
                x =>
                    new AtgenElementBonus()
                    {
                        elemental_type = x,
                        hp = 0,
                        attack = 0
                    }
            )
            .ToDictionary(x => x.elemental_type, x => x);

        foreach (int id in buildIds)
        {
            FortPlantDetail d = MasterAsset.FortPlant.Get(id);

            if (d.EffectId != FortEffectTypes.Element)
                continue;

            result[(UnitElement)d.EffType1].hp += d.EffArgs1;
            result[(UnitElement)d.EffType1].attack += d.EffArgs2;
#if CHEATING
            result[(UnitElement)d.EffType1].attack += 100;
#endif

            if (d.EffType2 != 0)
            {
                result[(UnitElement)d.EffType2].hp += d.EffArgs1;
                result[(UnitElement)d.EffType2].attack += d.EffArgs2;

#if CHEATING
                result[(UnitElement)d.EffType2].attack += 100;
#endif
            }
        }

        return result.Select(x => x.Value);
    }

    private static IEnumerable<AtgenParamBonus> GetFortParamBonus(IEnumerable<int> buildIds)
    {
        IDictionary<WeaponTypes, AtgenParamBonus> result = Enum.GetValues<WeaponTypes>()
            .Select(
                x =>
                    new AtgenParamBonus()
                    {
                        weapon_type = x,
                        hp = 0,
                        attack = 0
                    }
            )
            .ToDictionary(x => x.weapon_type, x => x);

        foreach (int id in buildIds)
        {
            FortPlantDetail d = MasterAsset.FortPlant.Get(id);

            if (d.EffectId != FortEffectTypes.Weapon)
                continue;

            result[(WeaponTypes)d.EffType1].hp += d.EffArgs1;
            result[(WeaponTypes)d.EffType1].attack += d.EffArgs2;
#if CHEATING
            result[(WeaponTypes)d.EffType1].attack += 100;
#endif

            if (d.EffType2 != 0)
            {
                result[(WeaponTypes)d.EffType2].hp += d.EffArgs1;
                result[(WeaponTypes)d.EffType2].attack += d.EffArgs2;
#if CHEATING
                result[(WeaponTypes)d.EffType2].attack += 100;
#endif
            }
        }

        return result.Select(x => x.Value);
    }

    private static IEnumerable<AtgenDragonBonus> GetFortDragonBonus(IEnumerable<int> buildIds)
    {
        IDictionary<UnitElement, AtgenDragonBonus> result = Enum.GetValues<UnitElement>()
            .Select(
                x =>
                    new AtgenDragonBonus()
                    {
                        elemental_type = x,
                        hp = 0,
                        attack = 0,
                        dragon_bonus = 0,
                    }
            )
            .ToDictionary(x => x.elemental_type, x => x);

        foreach (int id in buildIds)
        {
            FortPlantDetail d = MasterAsset.FortPlant.Get(id);

            if (d.EffectId == FortEffectTypes.DragonStats)
            {
                result[(UnitElement)d.EffType1].hp += d.EffArgs1;
                result[(UnitElement)d.EffType1].attack += d.EffArgs2;
#if CHEATING
                result[(UnitElement)d.EffType1].attack += 100;
#endif

                if (d.EffType2 != 0)
                {
                    result[(UnitElement)d.EffType2].hp += d.EffArgs1;
                    result[(UnitElement)d.EffType2].attack += d.EffArgs2;
#if CHEATING
                    result[(UnitElement)d.EffType2].attack += 100;
#endif
                }
            }
            else if (d.EffectId == FortEffectTypes.DragonDamage)
            {
                result[(UnitElement)d.EffType1].dragon_bonus += d.EffArgs1;
#if CHEATING
                result[(UnitElement)d.EffType1].dragon_bonus += 100;
#endif
                // No facility gives dragon bonus to two elemental types
            }
        }

        return result.Select(x => x.Value);
    }

    private static IEnumerable<AtgenParamBonus> GetWeaponParamBonus(
        IEnumerable<WeaponBodies> weaponBodyIds
    )
    {
        IDictionary<WeaponTypes, AtgenParamBonus> result = Enum.GetValues<WeaponTypes>()
            .Select(
                x =>
                    new AtgenParamBonus()
                    {
                        weapon_type = x,
                        hp = 0,
                        attack = 0
                    }
            )
            .ToDictionary(x => x.weapon_type, x => x);

        foreach (WeaponBodies id in weaponBodyIds)
        {
            WeaponBody w = MasterAsset.WeaponBody.Get(id);

            result[w.WeaponType].hp += w.WeaponPassiveEffHp;
            result[w.WeaponType].attack += w.WeaponPassiveEffAtk;
#if CHEATING
            result[w.WeaponType].attack += 100;
#endif
        }

        return result.Select(x => x.Value);
    }

    private static class StubData
    {
        // Source: https://dragalialost.wiki/w/Notte's_Notes#Encyclopedia
        public static readonly ImmutableList<AtgenElementBonus> MaxAlbumCharaBonus =
            ImmutableList<AtgenElementBonus>.Empty.AddRange(
                new List<AtgenElementBonus>()
                {
                    new()
                    {
                        elemental_type = UnitElement.Fire,
                        hp = 14.1f
#if CHEATING
                            * 100
#endif
                        ,
                        attack = 14.1f
#if CHEATING
                            * 100
#endif
                        ,
                    },
                    new()
                    {
                        elemental_type = UnitElement.Water,
                        hp = 13.8f,
                        attack = 13.8f
                    },
                    new()
                    {
                        elemental_type = UnitElement.Wind,
                        hp = 13.9f,
                        attack = 13.9f
                    },
                    new()
                    {
                        elemental_type = UnitElement.Light,
                        hp = 14.5f,
                        attack = 14.5f
                    },
                    new()
                    {
                        elemental_type = UnitElement.Dark,
                        hp = 13.3f,
                        attack = 13.3f
                    }
                }
            );

        // Source: https://dragalialost.wiki/w/Notte's_Notes#Encyclopedia
        public static readonly ImmutableList<AtgenElementBonus> MaxAlbumDragonBonus =
            ImmutableList<AtgenElementBonus>.Empty.AddRange(
                new List<AtgenElementBonus>()
                {
                    new()
                    {
                        elemental_type = UnitElement.Fire,
                        hp = 5.8f
#if CHEATING
                            * 100
#endif
                        ,
                        attack = 5f
#if CHEATING
                            * 100
#endif
                        ,
                    },
                    new()
                    {
                        elemental_type = UnitElement.Water,
                        hp = 5.4f,
                        attack = 4.6f
                    },
                    new()
                    {
                        elemental_type = UnitElement.Wind,
                        hp = 6.3f,
                        attack = 5.4f
                    },
                    new()
                    {
                        elemental_type = UnitElement.Light,
                        hp = 6.0f,
                        attack = 5.2f
                    },
                    new()
                    {
                        elemental_type = UnitElement.Dark,
                        hp = 7.3f,
                        attack = 6.4f
                    }
                }
            );
    }
}
