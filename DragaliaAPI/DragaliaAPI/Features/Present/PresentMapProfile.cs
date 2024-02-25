using AutoMapper;
using DragaliaAPI.AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Present;

public class PresentMapProfile : Profile
{
    public PresentMapProfile()
    {
        this.CreateMap<DbPlayerPresent, PresentDetailList>()
            .ForMember(
                x => x.ReceiveLimitTime,
                opts => opts.NullSubstitute(DateTimeOffset.UnixEpoch)
            )
            .ForMember(x => x.ExtraParameter1, opts => opts.Ignore())
            .ForMember(x => x.ExtraParameter2, opts => opts.Ignore())
            .ForMember(x => x.ExtraParameter3, opts => opts.Ignore())
            .ForMember(x => x.ExtraParameter4, opts => opts.Ignore())
            .ForMember(x => x.ExtraParameter5, opts => opts.Ignore());

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
            )
            .ForMember(x => x.CreateTime, opts => opts.Ignore());

        this.CreateMap<DbPlayerPresentHistory, PresentHistoryList>();

        this.DisableConstructorMapping();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
