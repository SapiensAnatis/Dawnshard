using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class InventoryReverseMapProfile : Profile
{
    public InventoryReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<MaterialList, DbPlayerMaterial>();
        this.CreateMap<DragonGiftList, DbPlayerDragonGift>();
        this.CreateMap<EquipStampList, DbEquippedStamp>();

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
