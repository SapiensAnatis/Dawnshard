using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class UserDataReverseMapProfile : Profile
{
    public UserDataReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<UserData, DbPlayerUserData>()
            .ForMember(x => x.TutorialFlag, opts => opts.Ignore()) // Mapped from TutorialFlagList
            .ForMember(x => x.LastSaveImportTime, opts => opts.Ignore())
            .ForMember(x => x.ActiveMemoryEventId, opts => opts.MapFrom(src => 0));
    }
}
