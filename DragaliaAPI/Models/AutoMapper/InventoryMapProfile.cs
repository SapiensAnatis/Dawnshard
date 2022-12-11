using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class InventoryMapProfile : Profile
{
    public InventoryMapProfile()
    {
        this.CreateMap<DbPlayerMaterial, MaterialList>();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
