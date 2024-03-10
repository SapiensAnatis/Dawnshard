using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.AutoMapper.Profiles;

public class QuestReverseMapProfile : Profile
{
    public QuestReverseMapProfile()
    {
        this.AddGlobalIgnore("ViewerId");
        this.AddGlobalIgnore("Owner");

        this.CreateMap<QuestStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(x => x.QuestStoryId))
            .ForMember(x => x.StoryType, o => o.MapFrom(src => StoryTypes.Quest));

        this.CreateMap<UnitStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(src => src.UnitStoryId))
            .ForMember(
                x => x.State,
                o => o.MapFrom(src => src.IsRead ? StoryState.Read : StoryState.Unlocked)
            )
            .ForMember(x => x.StoryType, o => o.MapFrom<UnitStoryTypeResolver>());

        this.CreateMap<CastleStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(src => src.CastleStoryId))
            .ForMember(
                x => x.State,
                o => o.MapFrom(src => src.IsRead ? StoryState.Read : StoryState.Unlocked)
            )
            .ForMember(x => x.StoryType, o => o.MapFrom(src => StoryTypes.Castle));

        this.CreateMap<DmodeStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(src => src.DmodeStoryId))
            .ForMember(
                x => x.State,
                o => o.MapFrom(src => src.IsRead ? StoryState.Read : StoryState.Unlocked)
            )
            .ForMember(x => x.StoryType, o => o.MapFrom(src => StoryTypes.DungeonMode));
    }
}
