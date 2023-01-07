using AutoMapper;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Models.AutoMapper;

public class HelperMapProfile : Profile
{
    public HelperMapProfile()
    {
        this.CreateMap<AtgenSupportChara, CharaList>()
            .ForMember(x => x.exp, opts => opts.Ignore())
            .ForMember(x => x.is_new, opts => opts.Ignore())
            .ForMember(x => x.burst_attack_level, opts => opts.Ignore())
            .ForMember(x => x.limit_break_count, opts => opts.Ignore())
            .ForMember(x => x.combo_buildup_count, opts => opts.Ignore())
            .ForMember(x => x.gettime, opts => opts.Ignore())
            .ForMember(x => x.mana_circle_piece_id_list, opts => opts.Ignore())
            .ForMember(x => x.is_temporary, opts => opts.Ignore())
            .ForMember(x => x.list_view_flag, opts => opts.Ignore());

        this.CreateMap<AtgenSupportDragon, DragonList>()
            .ForMember(x => x.exp, opts => opts.Ignore())
            .ForMember(x => x.is_lock, opts => opts.Ignore())
            .ForMember(x => x.is_new, opts => opts.Ignore())
            .ForMember(x => x.get_time, opts => opts.Ignore());

        /*
        this.CreateMap<AtgenSupportWeapon, WeaponList>()
            .ForMember(x => x.is_lock, opts => opts.Ignore())
            .ForMember(x => x.is_new, opts => opts.Ignore())
            .ForMember(x => x.gettime, opts => opts.Ignore())
            .ForMember(x => x.exp, opts => opts.Ignore());
        
        this.CreateMap<AtgenSupportAmulet, AmuletList>()
            .ForMember(x => x.is_lock, opts => opts.Ignore())
            .ForMember(x => x.is_new, opts => opts.Ignore())
            .ForMember(x => x.gettime, opts => opts.Ignore())
            .ForMember(x => x.exp, opts => opts.Ignore());
        */

        this.CreateMap<AtgenSupportWeaponBody, GameWeaponBody>()
            .ForMember(
                x => x.weapon_body_id,
                opts => opts.MapFrom(x => (WeaponBodies)x.weapon_body_id)
            )
            .ForMember(x => x.skill_no, opts => opts.Ignore())
            .ForMember(x => x.skill_level, opts => opts.Ignore())
            .ForMember(x => x.ability_1_level, opts => opts.Ignore())
            .ForMember(x => x.ability_2_level, opts => opts.Ignore());

        this.CreateMap<AtgenSupportCrestSlotType1List, GameAbilityCrest>()
            .ForMember(
                x => x.ability_crest_id,
                opts => opts.MapFrom(x => (AbilityCrests)x.ability_crest_id)
            )
            .ForMember(x => x.ability_1_level, opts => opts.Ignore())
            .ForMember(x => x.ability_2_level, opts => opts.Ignore());

        this.CreateMap<AtgenSupportTalisman, TalismanList>()
            .ForMember(x => x.is_new, opts => opts.Ignore())
            .ForMember(x => x.is_lock, opts => opts.Ignore())
            .ForMember(x => x.gettime, opts => opts.Ignore());

        this.SourceMemberNamingConvention = ExactMatchNamingConvention.Instance;
        this.DestinationMemberNamingConvention = ExactMatchNamingConvention.Instance;
    }
}
