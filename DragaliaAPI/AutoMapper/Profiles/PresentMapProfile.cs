using AutoMapper;
using AutoMapper.Internal;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class PresentMapProfile : Profile
{
    public PresentMapProfile()
    {
        this.CreateMap<DbPlayerPresent, PresentDetailList>()
            .ForMember(
                x => x.receive_limit_time,
                opts => opts.NullSubstitute(DateTimeOffset.UnixEpoch)
            );

        this.CreateMap<DbPlayerPresent, DbPlayerPresentHistory>()
            .ForMember(
                nameof(DbPlayerPresentHistory.CreateTime),
                x => x.MapFrom(src => DateTimeOffset.UtcNow)
            )
            .ForSourceMember(nameof(DbPlayerPresent.ReceiveLimitTime), opts => opts.DoNotValidate())
            .ForSourceMember(nameof(DbPlayerPresent.MasterId), opts => opts.DoNotValidate())
            .ForSourceMember(nameof(DbPlayerPresent.State), opts => opts.DoNotValidate())
            .ForMember(
                nameof(DbPlayerPresentHistory.Id),
                x => x.MapFrom(nameof(DbPlayerPresent.PresentId))
            );

        this.CreateMap<DbPlayerPresentHistory, PresentHistoryList>();

        this.DisableConstructorMapping();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
