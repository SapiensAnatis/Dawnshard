using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class ItemMapProfile : Profile
{
    public ItemMapProfile()
    {
        CreateMap<DbPlayerUseItem, ItemList>();

        SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
