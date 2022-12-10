using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Models.AutoMapper;

public class QuestMapProfile : Profile
{
    public QuestMapProfile()
    {
        this.CreateMap<DbQuest, QuestList>().ReverseMap();

        this.CreateMap<DbPlayerStoryState, QuestStoryList>()
            .ForMember(x => x.quest_story_id, o => o.MapFrom(nameof(DbPlayerStoryState.StoryId)));

        this.CreateMap<DbPlayerStoryState, UnitStoryList>()
            .ForMember(x => x.unit_story_id, o => o.MapFrom(src => src.StoryId))
            .ForMember(x => x.is_read, o => o.MapFrom(src => src.State));

        this.CreateMap<DbPlayerStoryState, CastleStoryList>()
            .ForMember(x => x.castle_story_id, o => o.MapFrom(src => src.StoryId))
            .ForMember(x => x.is_read, o => o.MapFrom(src => src.State));

        this.CreateMap<DataQuestAreaInfo, AreaInfoList>();

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
