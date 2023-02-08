using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class SummonMapProfile : Profile
{
    public SummonMapProfile()
    {
        this.CreateMap<DbPlayerSummonHistory, SummonHistoryList>()
            .ForMember(nameof(SummonHistoryList.summon_point_id), o => o.MapFrom(x => x.SummonId));

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
