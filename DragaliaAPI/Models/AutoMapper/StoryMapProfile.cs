using AutoMapper;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

namespace DragaliaAPI.Models.AutoMapper;

public class StoryMapProfile : Profile
{
    public StoryMapProfile()
    {
        this.CreateMap<DbPlayerStoryState, QuestStory>()
            .ForMember<int>(x => x.quest_story_id, opts => opts.MapFrom<int>(x => x.StoryId));
    }
}
