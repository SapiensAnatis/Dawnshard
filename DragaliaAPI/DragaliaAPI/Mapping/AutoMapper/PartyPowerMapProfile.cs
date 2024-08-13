using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class PartyPowerMapProfile : Profile
{
    public PartyPowerMapProfile()
    {
        this.CreateMap<DbPartyPower, PartyPowerData>();
    }
}
