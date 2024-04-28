using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

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
