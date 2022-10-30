using AutoMapper;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

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
