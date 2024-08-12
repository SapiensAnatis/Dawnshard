﻿using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class FortReverseMapProfile : Profile
{
    public FortReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<BuildList, DbFortBuild>()
            .ForMember(x => x.LastIncomeDate, opts => opts.MapFrom(src => DateTime.UnixEpoch))
            .ForMember(x => x.BuildId, opts => opts.Ignore());
    }
}
