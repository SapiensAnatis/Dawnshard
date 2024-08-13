using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class ItemMapProfile : Profile
{
    public ItemMapProfile()
    {
        this.CreateMap<DbPlayerUseItem, ItemList>();
    }
}
