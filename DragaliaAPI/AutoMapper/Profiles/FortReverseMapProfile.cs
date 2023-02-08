using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class FortReverseMapProfile : Profile
{
    public FortReverseMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<BuildList, DbFortBuild>()
            .ForMember(x => x.LastIncomeDate, opts => opts.MapFrom(src => DateTime.UnixEpoch))
            .ForMember(x => x.BuildId, opts => opts.Ignore());

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
