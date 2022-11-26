using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class PartyMapProfile : Profile
{
    public PartyMapProfile()
    {
        this.CreateMap<DbParty, PartyList>()
            .ForMember(nameof(PartyList.party_setting_list), opts => opts.MapFrom(x => x.Units))
            .ReverseMap()
            .ForMember(x => x.Units, opts => opts.MapFrom(x => x.party_setting_list));

        this.CreateMap<DbPartyUnit, PartySettingList>()
            .ForMember(nameof(PartySettingList.equip_weapon_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_2_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_skin_weapon_id), opts => opts.Ignore())
            .ReverseMap();

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
