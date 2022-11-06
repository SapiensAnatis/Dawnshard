using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Models.AutoMapper;

public class UserDataMapProfile : Profile
{
    public UserDataMapProfile()
    {
        this.CreateMap<DbPlayerUserData, UserData>();

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
