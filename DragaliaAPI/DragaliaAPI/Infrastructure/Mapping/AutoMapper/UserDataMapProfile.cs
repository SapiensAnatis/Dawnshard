using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

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
