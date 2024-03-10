using AutoMapper;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class HelperMapProfile : Profile
{
    public HelperMapProfile()
    {
        this.CreateMap<AtgenSupportChara, CharaList>()
            .ForMember(x => x.Exp, opts => opts.Ignore())
            .ForMember(x => x.IsNew, opts => opts.Ignore())
            .ForMember(x => x.BurstAttackLevel, opts => opts.Ignore())
            .ForMember(x => x.LimitBreakCount, opts => opts.Ignore())
            .ForMember(x => x.ComboBuildupCount, opts => opts.Ignore())
            .ForMember(x => x.GetTime, opts => opts.Ignore())
            .ForMember(x => x.ManaCirclePieceIdList, opts => opts.Ignore())
            .ForMember(x => x.IsTemporary, opts => opts.Ignore())
            .ForMember(x => x.ListViewFlag, opts => opts.Ignore());

        this.CreateMap<AtgenSupportDragon, DragonList>()
            .ForMember(x => x.Exp, opts => opts.Ignore())
            .ForMember(x => x.IsLock, opts => opts.Ignore())
            .ForMember(x => x.IsNew, opts => opts.Ignore())
            .ForMember(x => x.GetTime, opts => opts.Ignore());

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
            .ForMember(x => x.WeaponBodyId, opts => opts.MapFrom(x => x.WeaponBodyId))
            .ForMember(x => x.SkillNo, opts => opts.Ignore())
            .ForMember(x => x.SkillLevel, opts => opts.Ignore())
            .ForMember(x => x.Ability1Level, opts => opts.Ignore())
            .ForMember(x => x.Ability2Level, opts => opts.Ignore());

        this.CreateMap<AtgenSupportCrestSlotType1List, GameAbilityCrest>()
            .ForMember(x => x.AbilityCrestId, opts => opts.MapFrom(x => x.AbilityCrestId))
            .ForMember(x => x.Ability1Level, opts => opts.Ignore())
            .ForMember(x => x.Ability2Level, opts => opts.Ignore());

        this.CreateMap<AtgenSupportTalisman, TalismanList>()
            .ForMember(x => x.IsNew, opts => opts.Ignore())
            .ForMember(x => x.IsLock, opts => opts.Ignore())
            .ForMember(x => x.GetTime, opts => opts.Ignore());
    }
}
