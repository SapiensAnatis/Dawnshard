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
            .ForMember(x => x.ability_1_level, opts => opts.MapFrom(x => x.AbilityLevel))
            .ForMember(x => x.ability_2_level, opts => opts.MapFrom(x => x.AbilityLevel));

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

        this.CreateMap<DbQuestClearPartyUnit, PartySettingList>()
            // Deprecated
            .ForMember(nameof(PartySettingList.equip_weapon_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_2_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_skin_weapon_id), opts => opts.Ignore());

        this.CreateMap<DbDetailedPartyUnit, PartyUnitList>();

        this.CreateMap<DbAbilityCrest, GameAbilityCrest>()
            .ForMember(x => x.ability_1_level, opts => opts.MapFrom(x => x.AbilityLevel))
            .ForMember(x => x.ability_2_level, opts => opts.MapFrom(x => x.AbilityLevel));

        this.CreateMap<DbWeaponSkin, GameWeaponSkin>();

        this.CreateMap<DbWeaponBody, GameWeaponBody>();

        this.CreateMap<DbWeaponPassiveAbility, WeaponPassiveAbilityList>();

        this.CreateMap<DbEditSkillData, EditSkillCharaData>();

        // Entirely manually mapped, yay
        this.CreateMap<DbDetailedPartyUnit, UserSupportList>()
            // Manually mapped
            .ForMember(x => x.viewer_id, opts => opts.Ignore())
            .ForMember(x => x.name, opts => opts.Ignore())
            .ForMember(x => x.level, opts => opts.Ignore())
            .ForMember(x => x.last_login_date, opts => opts.Ignore())
            .ForMember(x => x.emblem_id, opts => opts.Ignore())
            .ForMember(x => x.guild, opts => opts.Ignore())
            .ForMember(x => x.max_party_power, opts => opts.Ignore())
            // Renamed
            .ForMember(x => x.support_chara, opts => opts.MapFrom(y => y.CharaData))
            .ForMember(x => x.support_weapon_body, opts => opts.MapFrom(y => y.WeaponBodyData))
            .ForMember(x => x.support_dragon, opts => opts.MapFrom(y => y.DragonData))
            .ForMember(
                x => x.support_crest_slot_type_1_list,
                opts => opts.MapFrom(y => y.CrestSlotType1CrestList)
            )
            .ForMember(
                x => x.support_crest_slot_type_2_list,
                opts => opts.MapFrom(y => y.CrestSlotType2CrestList)
            )
            .ForMember(
                x => x.support_crest_slot_type_3_list,
                opts => opts.MapFrom(y => y.CrestSlotType3CrestList)
            )
            .ForMember(x => x.support_talisman, opts => opts.MapFrom(y => y.TalismanData))
            // Deprecated
            .ForMember(x => x.support_weapon, opts => opts.Ignore())
            .ForMember(x => x.support_amulet, opts => opts.Ignore())
            .ForMember(x => x.support_amulet_2, opts => opts.Ignore());

        this.CreateMap<DbPlayerCharaData, AtgenSupportChara>()
            // No idea what this is
            .ForMember(x => x.status_plus_count, opts => opts.Ignore());

        this.CreateMap<DbWeaponBody, AtgenSupportWeaponBody>();

        this.CreateMap<DbPlayerDragonData, AtgenSupportDragon>()
            // Seems important but wasn't required for dungeon_start?
            .ForMember(x => x.hp, opts => opts.Ignore())
            .ForMember(x => x.attack, opts => opts.Ignore())
            // No idea what this is
            .ForMember(x => x.status_plus_count, opts => opts.Ignore());

        this.CreateMap<DbAbilityCrest, AtgenSupportCrestSlotType1List>();

        this.CreateMap<DbTalisman, AtgenSupportTalisman>();

        this.DisableConstructorMapping();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
