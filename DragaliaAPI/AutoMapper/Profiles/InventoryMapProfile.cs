using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class InventoryMapProfile : Profile
{
    public InventoryMapProfile()
    {
        this.CreateMap<DbPlayerMaterial, MaterialList>();
        CreateMap<DbPlayerDragonGift, DragonGiftList>();
        this.CreateMap<DbEquippedStamp, EquipStampList>();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
