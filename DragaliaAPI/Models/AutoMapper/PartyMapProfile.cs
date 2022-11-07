using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Models.AutoMapper;

public class PartyMapProfile : Profile
{
    public PartyMapProfile()
    {
        this.CreateMap<DbParty, Party>()
            .ForCtorParam(nameof(Party.party_setting_list), opts => opts.MapFrom(x => x.Units));
        this.CreateMap<DbPartyUnit, PartyUnit>();

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
