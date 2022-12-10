using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class UnitReverseMapProfile : Profile
{
    public UnitReverseMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");

        this.CreateMap<DragonList, DbPlayerDragonData>()
            .ForMember(x => x.DragonKeyId, opts => opts.Ignore());

        this.CreateMap<CharaList, DbPlayerCharaData>()
            .ForMember(x => x.LimitBreakCount, opts => opts.Ignore());

        this.CreateMap<DragonReliabilityList, DbPlayerDragonReliability>()
            .ForMember(x => x.Level, opts => opts.MapFrom(src => src.reliability_level))
            .ForMember(x => x.Exp, opts => opts.MapFrom(src => src.reliability_total_exp));

        this.CreateMap<AbilityCrestList, DbAbilityCrest>();

        this.CreateMap<WeaponBodyList, DbWeaponBody>();

        this.CreateMap<PartyList, DbParty>()
            .ForMember(x => x.Units, opts => opts.MapFrom(src => src.party_setting_list));

        this.CreateMap<PartySettingList, DbPartyUnit>()
            // Do not want to transport primary keys from official servers to my db
            .ForMember(x => x.EquipDragonKeyId, opts => opts.MapFrom(src => 0))
            .ForMember(x => x.EquipTalismanKeyId, opts => opts.MapFrom(src => 0))
            // Auto-generated primary key
            .ForMember(x => x.Id, opts => opts.Ignore())
            // Foreign keys
            .ForMember(x => x.PartyNo, opts => opts.Ignore())
            .ForMember(x => x.Party, opts => opts.Ignore());

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
