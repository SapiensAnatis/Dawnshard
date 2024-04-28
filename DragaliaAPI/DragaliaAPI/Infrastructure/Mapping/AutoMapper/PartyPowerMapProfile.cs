using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

public class PartyPowerMapProfile : Profile
{
    public PartyPowerMapProfile()
    {
        this.CreateMap<DbPartyPower, PartyPowerData>();
    }
}
