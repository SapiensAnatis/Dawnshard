using AutoMapper;
using AutoMapper.Internal;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Models.AutoMapper;

public class PlayerMapProfile : Profile
{
    public PlayerMapProfile()
    {
        this.AllowNullDestinationValues = true;

        // We do not yet have source properties for all dest properties
        this.CreateMap<DbPlayer, LoadIndexData>()
            .ValidateMemberList(MemberList.Source)
            .ForMember(
                x => x.unit_story_list,
                opts => opts.MapFrom(x => x.StoryStates.Where(x => x.StoryType == StoryTypes.Chara))
            )
            .ForMember(
                x => x.castle_story_list,
                opts =>
                    opts.MapFrom(x => x.StoryStates.Where(x => x.StoryType == StoryTypes.Castle))
            )
            .ForMember(
                x => x.quest_story_list,
                opts => opts.MapFrom(x => x.StoryStates.Where(x => x.StoryType == StoryTypes.Quest))
            )
            .ForSourceMember(x => x.StoryStates, opts => opts.DoNotValidate())
            .ForSourceMember(x => x.AccountId, opts => opts.DoNotValidate())
            .ForSourceMember(x => x.UnitSets, opts => opts.DoNotValidate())
            .ForSourceMember(x => x.SummonHistory, opts => opts.DoNotValidate())
            .ForSourceMember(x => x.Currencies, opts => opts.DoNotValidate());

        this.SourceMemberNamingConvention = DatabaseNamingConvention.Instance;
        this.DestinationMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
    }
}
