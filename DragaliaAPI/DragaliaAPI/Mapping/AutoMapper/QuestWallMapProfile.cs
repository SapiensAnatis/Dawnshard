using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class QuestWallMapProfile : Profile
{
    public QuestWallMapProfile()
    {
        this.CreateMap<DbPlayerQuestWall, QuestWallList>().ReverseMap();
    }
}
