using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.DTO;

namespace DragaliaAPI.Infrastructure.AutoMapper;

public class QuestWallMapProfile : Profile
{
    public QuestWallMapProfile()
    {
        this.CreateMap<DbPlayerQuestWall, QuestWallList>().ReverseMap();
    }
}
