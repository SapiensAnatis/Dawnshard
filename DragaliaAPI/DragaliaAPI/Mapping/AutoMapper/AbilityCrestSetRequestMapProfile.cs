using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class AbilityCrestSetRequestMapProfile : Profile
{
    public AbilityCrestSetRequestMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<AbilityCrestSetAbilityCrestSetRequest, DbAbilityCrestSet>()
            .ForMember(
                x => x.CrestSlotType1CrestId1,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType1CrestId1)
            )
            .ForMember(
                x => x.CrestSlotType1CrestId2,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType1CrestId2)
            )
            .ForMember(
                x => x.CrestSlotType1CrestId3,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType1CrestId3)
            )
            .ForMember(
                x => x.CrestSlotType2CrestId1,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType2CrestId1)
            )
            .ForMember(
                x => x.CrestSlotType2CrestId2,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType2CrestId2)
            )
            .ForMember(
                x => x.CrestSlotType3CrestId1,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType3CrestId1)
            )
            .ForMember(
                x => x.CrestSlotType3CrestId2,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.CrestSlotType3CrestId2)
            )
            .ForMember(
                x => x.TalismanKeyId,
                opts => opts.MapFrom(src => src.RequestAbilityCrestSetData.TalismanKeyId)
            );
    }
}
