using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class InventoryMapProfile : Profile
{
    public InventoryMapProfile()
    {
        this.CreateMap<DbPlayerMaterial, MaterialList>();
        this.CreateMap<DbPlayerMaterial, ItemList>()
            .ForMember(x => x.item_id, opts => opts.MapFrom(src => src.MaterialId));
        CreateMap<DbPlayerDragonGift, DragonGiftList>();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
