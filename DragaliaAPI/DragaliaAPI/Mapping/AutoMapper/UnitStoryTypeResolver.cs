using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

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
        // Normally this would do a MasterAsset lookup to get the correct story type
        // based on the target chara/dragon id. However, this is slow, and all dragon ids start
        // at 200000000, meaning that we can just check for that here and decide based on that
        return source.UnitStoryId > 200000000 ? StoryTypes.Dragon : StoryTypes.Chara;
    }
}
