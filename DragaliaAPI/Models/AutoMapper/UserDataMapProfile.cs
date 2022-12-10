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
            .ReverseMap();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
