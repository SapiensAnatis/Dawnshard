using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class ItemMapProfile : Profile
{
    public ItemMapProfile()
    {
        this.CreateMap<DbPlayerUseItem, ItemList>();
    }
}
