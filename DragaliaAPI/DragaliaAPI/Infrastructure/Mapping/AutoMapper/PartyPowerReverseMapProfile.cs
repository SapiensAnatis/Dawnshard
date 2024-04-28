using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

public class PartyPowerReverseMapProfile : Profile
{
    public PartyPowerReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<PartyPowerData, DbPartyPower>();
    }
}
