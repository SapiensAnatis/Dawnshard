using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DragaliaAPI.Models.AutoMapper;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;

namespace DragaliaAPI.Test.Unit.AutoMapper;

public class AutoMapperProfileTest
{
    [Fact]
    public void QuestMapProfile_IsValid()
    {
        MapperConfiguration config = new(cfg => cfg.AddProfile(new QuestMapProfile()));
        config.Invoking(x => x.AssertConfigurationIsValid()).Should().NotThrow();
    }

    [Fact]
    public void StoryMapProfile_IsValid()
    {
        MapperConfiguration config = new(cfg => cfg.AddProfile(new StoryMapProfile()));
        config.Invoking(x => x.AssertConfigurationIsValid()).Should().NotThrow();
    }
}
