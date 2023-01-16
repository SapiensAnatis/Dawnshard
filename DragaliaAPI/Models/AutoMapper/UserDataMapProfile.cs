﻿using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class UserDataMapProfile : Profile
{
    public UserDataMapProfile()
    {
        this.CreateMap<DbPlayerUserData, UserData>()
            .ForMember(x => x.age_group, opts => opts.Ignore())
            .ForMember(x => x.max_amulet_quantity, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.max_weapon_quantity, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.is_optin, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.prologue_end_time, opts => opts.MapFrom(x => 0));

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
