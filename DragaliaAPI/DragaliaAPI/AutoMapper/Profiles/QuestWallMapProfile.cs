using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper.Profiles;

public class QuestWallMapProfile : Profile
{
    public QuestWallMapProfile()
    {
        this.CreateMap<DbPlayerQuestWall, QuestWallList>().ReverseMap();
    }
}
