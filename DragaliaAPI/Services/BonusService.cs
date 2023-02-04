using System.Collections.Immutable;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

/// <summary>
/// Service for fort, weapon, and album bonuses
/// </summary>
public class BonusService : IBonusService
{
    private readonly IFortRepository fortRepository;
    private readonly IWeaponRepository weaponRepository;

    public BonusService(IFortRepository fortRepository, IWeaponRepository weaponRepository)
    {
        this.fortRepository = fortRepository;
        this.weaponRepository = weaponRepository;
    }

    public async Task<FortBonusList> GetBonusList()
    {
        IEnumerable<int> buildIds = (
            await this.fortRepository.Builds.Select(x => new { x.PlantId, x.Level }).ToListAsync()
        ).Select(x => MasterAssetUtils.GetPlantDetailId(x.PlantId, x.Level));

        IEnumerable<WeaponBodies> weaponIds = await this.weaponRepository.WeaponBodies
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
            dragon_time_bonus = new() { dragon_time_bonus = 0 },
            all_bonus = new() { attack = 0, hp = 0 },
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

            if (d.EffType2 != 0)
            {
                result[(UnitElement)d.EffType2].hp += d.EffArgs1;
                result[(UnitElement)d.EffType2].attack += d.EffArgs2;
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

            if (d.EffType2 != 0)
            {
                result[(WeaponTypes)d.EffType2].hp += d.EffArgs1;
                result[(WeaponTypes)d.EffType2].attack += d.EffArgs2;
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

                if (d.EffType2 != 0)
                {
                    result[(UnitElement)d.EffType2].hp += d.EffArgs1;
                    result[(UnitElement)d.EffType2].attack += d.EffArgs2;
                }
            }
            else if (d.EffectId == FortEffectTypes.DragonDamage)
            {
                result[(UnitElement)d.EffType1].dragon_bonus += d.EffArgs1;
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
                        hp = 14.1f,
                        attack = 14.1f
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
                        hp = 5.8f,
                        attack = 5f
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
