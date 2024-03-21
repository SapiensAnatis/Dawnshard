using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class UserDataMapProfile : Profile
{
    public UserDataMapProfile()
    {
        this.CreateMap<DbPlayerUserData, UserData>()
            .ForMember(x => x.AgeGroup, opts => opts.Ignore())
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.MaxAmuletQuantity, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.MaxWeaponQuantity, opts => opts.MapFrom(x => 0))
#pragma warning restore CS0612 // Type or member is obsolete
            .ForMember(x => x.IsOptin, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.PrologueEndTime, opts => opts.MapFrom(x => DateTimeOffset.UnixEpoch));
    }
}
