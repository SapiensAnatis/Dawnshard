using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class EmblemReverseMapProfile : Profile
{
    public EmblemReverseMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<EmblemList, DbEmblem>()
            .ForMember(x => x.EmblemId, o => o.MapFrom(x => x.emblem_id))
            .ForMember(x => x.GetTime, o => o.MapFrom(src => src.gettime))
            .ForMember(x => x.IsNew, o => o.MapFrom(src => src.is_new));

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
