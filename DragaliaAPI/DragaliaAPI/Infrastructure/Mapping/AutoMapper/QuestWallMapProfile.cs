using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Infrastructure.Mapping.AutoMapper;

public class QuestWallMapProfile : Profile
{
    public QuestWallMapProfile()
    {
        this.CreateMap<DbPlayerQuestWall, QuestWallList>().ReverseMap();
    }
}
