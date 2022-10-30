using AutoMapper;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

namespace DragaliaAPI.Models.AutoMapper;

public class QuestMapProfile : Profile
{
    public QuestMapProfile()
    {
        this.CreateMap<DbQuest, Quest>();

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
