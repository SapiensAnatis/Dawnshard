using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.AutoMapper;

public class UnitStoryTypeResolver : IValueResolver<UnitStoryList, DbPlayerStoryState, StoryTypes>
{
    public StoryTypes Resolve(
        UnitStoryList source,
        DbPlayerStoryState destination,
        StoryTypes destMember,
        ResolutionContext context
    )
    {
        return MasterAsset.UnitStory[source.unit_story_id].Type;
    }
}
