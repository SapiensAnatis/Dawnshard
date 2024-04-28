using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class InventoryMapProfile : Profile
{
    public InventoryMapProfile()
    {
        this.CreateMap<DbPlayerMaterial, MaterialList>();
        this.CreateMap<DbPlayerDragonGift, DragonGiftList>();
        this.CreateMap<DbEquippedStamp, EquipStampList>();
    }
}
