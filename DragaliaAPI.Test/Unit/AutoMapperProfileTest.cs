using System.Formats.Tar;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.AutoMapper;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Unit;

public class AutoMapperProfileTest
{
    public class AutoMapperTheoryData : TheoryData<Type>
    {
        public AutoMapperTheoryData()
        {
            IEnumerable<Type> types = typeof(Program).Assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Profile)));

            foreach (Type t in types)
                this.Add(t);
        }
    }

    [Theory]
    [ClassData(typeof(AutoMapperTheoryData))]
    public void Profile_IsValid(Type profileType)
    {
        Profile instance =
            (Profile?)Activator.CreateInstance(profileType)
            ?? throw new NullReferenceException($"Failed to create instance of {profileType.Name}");

        MapperConfiguration config = new(cfg => cfg.AddProfile(instance));

        config.Invoking(x => x.AssertConfigurationIsValid()).Should().NotThrow();
    }
}
