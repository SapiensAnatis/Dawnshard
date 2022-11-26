using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class PartyMapProfile : Profile
{
    public PartyMapProfile()
    {
        this.CreateMap<DbParty, PartyList>()
            .ForCtorParam(nameof(PartyList.party_setting_list), opts => opts.MapFrom(x => x.Units))
            .ReverseMap()
            .ForMember(x => x.Units, opts => opts.MapFrom(x => x.party_setting_list));
        this.CreateMap<DbPartyUnit, PartySettingList>().ReverseMap();

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
