using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class UserDataMapProfile : Profile
{
    public UserDataMapProfile()
    {
        this.CreateMap<DbPlayerUserData, UserData>()
            .ForMember<int>(x => x.AgeGroup, opts => opts.Ignore())
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(x => x.MaxAmuletQuantity, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.MaxWeaponQuantity, opts => opts.MapFrom(x => 0))
            .ForMember<bool>(x => x.IsOptin, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.PrologueEndTime, opts => opts.MapFrom(x => DateTimeOffset.UnixEpoch));
    }
}
