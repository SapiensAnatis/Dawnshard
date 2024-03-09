using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.AutoMapper.Profiles;

public class QuestMapProfile : Profile
{
    public QuestMapProfile()
    {
        this.CreateMap<DbQuest, QuestList>().ReverseMap();

        this.CreateMap<DbQuestEvent, QuestEventList>().ReverseMap();

        this.CreateMap<DbQuestTreasureList, QuestTreasureList>().ReverseMap();

        this.CreateMap<DbPlayerStoryState, QuestStoryList>()
            .ForMember(x => x.QuestStoryId, o => o.MapFrom(nameof(DbPlayerStoryState.StoryId)));

        this.CreateMap<DbPlayerStoryState, UnitStoryList>()
            .ForMember(x => x.UnitStoryId, o => o.MapFrom(src => src.StoryId))
            .ForMember(x => x.IsRead, o => o.MapFrom(src => src.State == StoryState.Read));

        this.CreateMap<DbPlayerStoryState, CastleStoryList>()
            .ForMember(x => x.CastleStoryId, o => o.MapFrom(src => src.StoryId))
            .ForMember(x => x.IsRead, o => o.MapFrom(src => src.State == StoryState.Read));

        this.CreateMap<DbPlayerStoryState, DmodeStoryList>()
            .ForMember(x => x.DmodeStoryId, o => o.MapFrom(src => src.StoryId))
            .ForMember(x => x.IsRead, o => o.MapFrom(src => src.State == StoryState.Read));

        this.CreateMap<AreaInfo, AreaInfoList>();
    }
}
