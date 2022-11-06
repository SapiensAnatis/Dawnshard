using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Models.AutoMapper;

public class StoryMapProfile : Profile
{
    public StoryMapProfile()
    {
        this.CreateMap<DbPlayerStoryState, QuestStory>()
            .ForCtorParam(
                nameof(QuestStory.quest_story_id),
                o => o.MapFrom(nameof(DbPlayerStoryState.StoryId))
            );
    }
}
