using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class PartyPowerMapProfile : Profile
{
    public PartyPowerMapProfile()
    {
        this.CreateMap<DbPartyPower, PartyPowerData>();
    }
}
