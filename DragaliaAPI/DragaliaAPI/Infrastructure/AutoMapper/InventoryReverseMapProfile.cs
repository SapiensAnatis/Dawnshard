using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class InventoryReverseMapProfile : Profile
{
    public InventoryReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<MaterialList, DbPlayerMaterial>();
        this.CreateMap<DragonGiftList, DbPlayerDragonGift>();
        this.CreateMap<EquipStampList, DbEquippedStamp>();
    }
}
