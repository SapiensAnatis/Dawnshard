using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class EmblemMapProfile : Profile
{
    public EmblemMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<DbEmblem, EmblemList>()
            .ForMember(x => x.emblem_id, o => o.MapFrom(x => x.EmblemId))
            .ForMember(x => x.gettime, o => o.MapFrom(src => src.GetTime))
            .ForMember(x => x.is_new, o => o.MapFrom(src => src.IsNew));

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
