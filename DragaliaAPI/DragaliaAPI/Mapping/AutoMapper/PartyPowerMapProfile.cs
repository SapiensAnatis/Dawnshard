using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class PartyPowerMapProfile : Profile
{
    public PartyPowerMapProfile()
    {
        this.CreateMap<DbPartyPower, PartyPowerData>();
    }
}
