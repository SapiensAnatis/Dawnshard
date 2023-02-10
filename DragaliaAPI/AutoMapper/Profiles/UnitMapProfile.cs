using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, DragonList>()
            .ForMember(x => x.status_plus_count, opts => opts.Ignore());

        this.CreateMap<DbPlayerCharaData, CharaList>()
            .ForMember(x => x.status_plus_count, opts => opts.Ignore());

        this.CreateMap<DbPlayerDragonReliability, DragonReliabilityList>()
            .ForMember(
                nameof(DragonReliabilityList.reliability_level),
                o => o.MapFrom(src => src.Level)
            )
            .ForMember(
                nameof(DragonReliabilityList.reliability_total_exp),
                o => o.MapFrom(src => src.Exp)
            );

        this.CreateMap<DbAbilityCrest, AbilityCrestList>()
            .ForMember(x => x.ability_1_level, opts => opts.MapFrom(x => 3))
            .ForMember(x => x.ability_2_level, opts => opts.MapFrom(x => 3));

        this.CreateMap<DbWeaponBody, WeaponBodyList>()
            // These members do not appear in the savefile
            .ForMember(x => x.ability_1_level, opts => opts.Ignore())
            .ForMember(x => x.ability_2_levell, opts => opts.Ignore())
            .ForMember(x => x.skill_no, opts => opts.Ignore())
            .ForMember(x => x.skill_level, opts => opts.Ignore());

        this.CreateMap<DbWeaponSkin, WeaponSkinList>();

        this.CreateMap<DbParty, PartyList>()
            .ForMember(nameof(PartyList.party_setting_list), opts => opts.MapFrom(x => x.Units));

        this.CreateMap<DbTalisman, TalismanList>();

        this.CreateMap<DbPartyUnit, PartySettingList>()
            // Deprecated
            .ForMember(nameof(PartySettingList.equip_weapon_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_2_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_skin_weapon_id), opts => opts.Ignore());

        this.CreateMap<DbDetailedPartyUnit, PartyUnitList>();

        this.CreateMap<DbAbilityCrest, GameAbilityCrest>()
            .ForMember(x => x.ability_1_level, opts => opts.MapFrom(x => 3))
            .ForMember(x => x.ability_2_level, opts => opts.MapFrom(x => 3));

        this.CreateMap<DbWeaponSkin, GameWeaponSkin>();

        this.CreateMap<DbWeaponBody, GameWeaponBody>()
            .ForMember(
                x => x.skill_no,
                opts => opts.MapFrom<GameWeaponBodyResolvers.SkillNoResolver>()
            )
            .ForMember(
                x => x.skill_level,
                opts => opts.MapFrom<GameWeaponBodyResolvers.SkillLevelResolver>()
            )
            .ForMember(
                x => x.ability_1_level,
                opts => opts.MapFrom<GameWeaponBodyResolvers.AbilityOneResolver>()
            )
            .ForMember(
                x => x.ability_2_level,
                opts => opts.MapFrom<GameWeaponBodyResolvers.AbilityTwoResolver>()
            );

        this.CreateMap<DbWeaponPassiveAbility, WeaponPassiveAbilityList>();

        this.CreateMap<DbEditSkillData, EditSkillCharaData>();

        this.DisableConstructorMapping();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
