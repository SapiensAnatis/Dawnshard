using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Game;

namespace DragaliaAPI.AutoMapper.Profiles;

public class AbilityCrestSetRequestMapProfile : Profile
{
    public AbilityCrestSetRequestMapProfile()
    {
        this.AddGlobalIgnore("DeviceAccount");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<AbilityCrestSetAbilityCrestSetRequest, DbAbilityCrestSet>()
            .ForMember(
                x => x.CrestSlotType1CrestId1,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_1_crest_id_1
                    )
            )
            .ForMember(
                x => x.CrestSlotType1CrestId2,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_1_crest_id_2
                    )
            )
            .ForMember(
                x => x.CrestSlotType1CrestId3,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_1_crest_id_3
                    )
            )
            .ForMember(
                x => x.CrestSlotType2CrestId1,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_2_crest_id_1
                    )
            )
            .ForMember(
                x => x.CrestSlotType2CrestId2,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_2_crest_id_2
                    )
            )
            .ForMember(
                x => x.CrestSlotType3CrestId1,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_3_crest_id_1
                    )
            )
            .ForMember(
                x => x.CrestSlotType3CrestId2,
                opts =>
                    opts.MapFrom(
                        src => src.request_ability_crest_set_data.crest_slot_type_3_crest_id_2
                    )
            )
            .ForMember(
                x => x.TalismanKeyId,
                opts => opts.MapFrom(src => src.request_ability_crest_set_data.talisman_key_id)
            );

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
