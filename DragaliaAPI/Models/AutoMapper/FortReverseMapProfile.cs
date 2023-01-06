using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class FortReverseMapProfile : Profile
{
    public FortReverseMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");

        this.CreateMap<BuildList, DbFortBuild>()
            .ForMember(
                x => x.LastIncomeDate,
                opts => opts.MapFrom(src => DateTime.UtcNow - src.last_income_time)
            );

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
