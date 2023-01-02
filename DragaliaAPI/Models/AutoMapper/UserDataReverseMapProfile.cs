﻿using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class UserDataReverseMapProfile : Profile
{
    public UserDataReverseMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");

        this.CreateMap<UserData, DbPlayerUserData>()
            .ForMember(x => x.TutorialFlag, opts => opts.Ignore()) // Mapped from TutorialFlagList
            .ForMember(x => x.LastSaveImportTime, opts => opts.Ignore());

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
