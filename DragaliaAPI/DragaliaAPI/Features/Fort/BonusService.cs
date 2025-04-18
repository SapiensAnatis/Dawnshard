﻿//#define CHEATS
#if CHEATS && DEBUG
#define CHEATING
#endif

using System.Collections.Immutable;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Fort;

/// <summary>
/// Service for fort, weapon, and album bonuses
/// </summary>
public class BonusService(
    IFortRepository fortRepository,
    IWeaponRepository weaponRepository,
    IPlayerIdentityService playerIdentityService,
    ILogger<BonusService> logger
) : IBonusService
{
    public Task<FortBonusList> GetBonusList(CancellationToken cancellationToken = default) =>
        this.GetBonusList(playerIdentityService.ViewerId, cancellationToken);

    public async Task<FortBonusList> GetBonusList(
        long viewerId,
        CancellationToken cancellationToken = default
    )
    {
        List<int> buildIds = (
            await fortRepository
                .Builds.IgnoreQueryFilters()
                .Where(x => x.ViewerId == viewerId && x.Level != 0)
                .Select(x => new { x.PlantId, x.Level })
                .ToListAsync(cancellationToken)
        )
            .Select(x => MasterAssetUtils.GetPlantDetailId(x.PlantId, x.Level))
            .ToList();

        List<WeaponBodies> weaponIds = await weaponRepository
            .WeaponBodies.IgnoreQueryFilters()
            .Where(x => x.ViewerId == viewerId && x.FortPassiveCharaWeaponBuildupCount != 0)
            .Select(x => x.WeaponBodyId)
            .ToListAsync(cancellationToken);

        return new()
        {
            ParamBonus = GetFortParamBonus(buildIds),
            ElementBonus = GetFortElementBonus(buildIds),
            DragonBonus = GetFortDragonBonus(buildIds),
            ParamBonusByWeapon = GetWeaponParamBonus(weaponIds),
            // TODO: Implement these bonuses
            CharaBonusByAlbum = StubData.MaxAlbumCharaBonus,
            DragonBonusByAlbum = StubData.MaxAlbumDragonBonus,
            // These are all 0 on my endgame save. Unsure of what, if anything, may increase them
            DragonTimeBonus = new()
            {
#if CHEATING
                DragonTimeBonus = 20
#endif
            },
            AllBonus = new()
            {
#if CHEATING
                Attack = 100,
                Hp = 100
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

        int level = await fortRepository
            .Builds.Where(x => x.PlantId == eventData.EventFortId)
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
            EventEffect = detail.EventEffectType,
            EffectValue = detail.EventEffectArgs,
        };
    }

    private static IEnumerable<AtgenElementBonus> GetFortElementBonus(List<int> buildIds)
    {
        Dictionary<UnitElement, AtgenElementBonus> result = Enum.GetValues<UnitElement>()
            .Select(x => new AtgenElementBonus()
            {
                ElementalType = x,
                Hp = 0,
                Attack = 0,
            })
            .ToDictionary(x => x.ElementalType, x => x);

        foreach (int id in buildIds)
        {
            FortPlantDetail d = MasterAsset.FortPlantDetail.Get(id);

            if (d.EffectId != FortEffectTypes.Element)
                continue;

            result[(UnitElement)d.EffType1].Hp += d.EffArgs1;
            result[(UnitElement)d.EffType1].Attack += d.EffArgs2;
#if CHEATING
            result[(UnitElement)d.EffType1].Attack += 100;
#endif

            if (d.EffType2 != 0)
            {
                result[(UnitElement)d.EffType2].Hp += d.EffArgs1;
                result[(UnitElement)d.EffType2].Attack += d.EffArgs2;

#if CHEATING
                result[(UnitElement)d.EffType2].Attack += 100;
#endif
            }
        }

        return result.Select(x => x.Value);
    }

    private static IEnumerable<AtgenParamBonus> GetFortParamBonus(List<int> buildIds)
    {
        Dictionary<WeaponTypes, AtgenParamBonus> result = Enum.GetValues<WeaponTypes>()
            .Select(x => new AtgenParamBonus()
            {
                WeaponType = x,
                Hp = 0,
                Attack = 0,
            })
            .ToDictionary(x => x.WeaponType, x => x);

        foreach (int id in buildIds)
        {
            FortPlantDetail d = MasterAsset.FortPlantDetail.Get(id);

            if (d.EffectId != FortEffectTypes.Weapon)
                continue;

            result[(WeaponTypes)d.EffType1].Hp += d.EffArgs1;
            result[(WeaponTypes)d.EffType1].Attack += d.EffArgs2;
#if CHEATING
            result[(WeaponTypes)d.EffType1].Attack += 100;
#endif

            if (d.EffType2 != 0)
            {
                result[(WeaponTypes)d.EffType2].Hp += d.EffArgs1;
                result[(WeaponTypes)d.EffType2].Attack += d.EffArgs2;
#if CHEATING
                result[(WeaponTypes)d.EffType2].Attack += 100;
#endif
            }
        }

        return result.Select(x => x.Value);
    }

    private static IEnumerable<AtgenDragonBonus> GetFortDragonBonus(List<int> buildIds)
    {
        Dictionary<UnitElement, AtgenDragonBonus> result = Enum.GetValues<UnitElement>()
            .Select(x => new AtgenDragonBonus()
            {
                ElementalType = x,
                Hp = 0,
                Attack = 0,
                DragonBonus = 0,
            })
            .ToDictionary(x => x.ElementalType, x => x);

        foreach (int id in buildIds)
        {
            FortPlantDetail d = MasterAsset.FortPlantDetail.Get(id);

            if (d.EffectId == FortEffectTypes.DragonStats)
            {
                result[(UnitElement)d.EffType1].Hp += d.EffArgs1;
                result[(UnitElement)d.EffType1].Attack += d.EffArgs2;
#if CHEATING
                result[(UnitElement)d.EffType1].Attack += 100;
#endif

                if (d.EffType2 != 0)
                {
                    result[(UnitElement)d.EffType2].Hp += d.EffArgs1;
                    result[(UnitElement)d.EffType2].Attack += d.EffArgs2;
#if CHEATING
                    result[(UnitElement)d.EffType2].Attack += 100;
#endif
                }
            }
            else if (d.EffectId == FortEffectTypes.DragonDamage)
            {
                result[(UnitElement)d.EffType1].DragonBonus += d.EffArgs1;
#if CHEATING
                result[(UnitElement)d.EffType1].DragonBonus += 100;
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
        Dictionary<WeaponTypes, AtgenParamBonus> result = Enum.GetValues<WeaponTypes>()
            .Select(x => new AtgenParamBonus()
            {
                WeaponType = x,
                Hp = 0,
                Attack = 0,
            })
            .ToDictionary(x => x.WeaponType, x => x);

        foreach (WeaponBodies id in weaponBodyIds)
        {
            WeaponBody w = MasterAsset.WeaponBody.Get(id);

            result[w.WeaponType].Hp += w.WeaponPassiveEffHp;
            result[w.WeaponType].Attack += w.WeaponPassiveEffAtk;
#if CHEATING
            result[w.WeaponType].Attack += 100;
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
                        ElementalType = UnitElement.Fire,
                        Hp = 14.1f
#if CHEATING
                            * 100
#endif
                        ,
                        Attack = 14.1f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Water,
                        Hp = 13.8f,
                        Attack = 13.8f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Wind,
                        Hp = 13.9f,
                        Attack = 13.9f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Light,
                        Hp = 14.5f,
                        Attack = 14.5f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Dark,
                        Hp = 13.3f,
                        Attack = 13.3f,
                    },
                }
            );

        // Source: https://dragalialost.wiki/w/Notte's_Notes#Encyclopedia
        public static readonly ImmutableList<AtgenElementBonus> MaxAlbumDragonBonus =
            ImmutableList<AtgenElementBonus>.Empty.AddRange(
                new List<AtgenElementBonus>()
                {
                    new()
                    {
                        ElementalType = UnitElement.Fire,
                        Hp = 5.8f
#if CHEATING
                            * 100
#endif
                        ,
                        Attack = 5f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Water,
                        Hp = 5.4f,
                        Attack = 4.6f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Wind,
                        Hp = 6.3f,
                        Attack = 5.4f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Light,
                        Hp = 6.0f,
                        Attack = 5.2f,
                    },
                    new()
                    {
                        ElementalType = UnitElement.Dark,
                        Hp = 7.3f,
                        Attack = 6.4f,
                    },
                }
            );
    }
}
