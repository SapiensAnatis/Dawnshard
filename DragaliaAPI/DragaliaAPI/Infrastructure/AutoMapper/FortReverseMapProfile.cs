using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class FortReverseMapProfile : Profile
{
    public FortReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<BuildList, DbFortBuild>()
            .ForMember(x => x.LastIncomeDate, opts => opts.MapFrom(src => DateTime.UnixEpoch))
            .ForMember(x => x.BuildId, opts => opts.Ignore());
    }
}
