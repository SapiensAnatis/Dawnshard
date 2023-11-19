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
            .ForMember(x => x.StoryId, o => o.MapFrom(x => x.quest_story_id))
            .ForMember(x => x.StoryType, o => o.MapFrom(src => StoryTypes.Quest));

        this.CreateMap<UnitStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(src => src.unit_story_id))
            .ForMember(x => x.State, o => o.MapFrom(src => src.is_read))
            .ForMember(x => x.StoryType, o => o.MapFrom<UnitStoryTypeResolver>());

        this.CreateMap<CastleStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(src => src.castle_story_id))
            .ForMember(x => x.State, o => o.MapFrom(src => src.is_read))
            .ForMember(x => x.StoryType, o => o.MapFrom(src => StoryTypes.Castle));

        this.CreateMap<DmodeStoryList, DbPlayerStoryState>()
            .ForMember(x => x.StoryId, o => o.MapFrom(src => src.dmode_story_id))
            .ForMember(x => x.State, o => o.MapFrom(src => src.is_read))
            .ForMember(x => x.StoryType, o => o.MapFrom(src => StoryTypes.DungeonMode));

        this.SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
        this.DestinationMemberNamingConvention = DatabaseNamingConvention.Instance;
    }
}
