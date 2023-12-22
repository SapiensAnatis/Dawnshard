using AutoMapper;

namespace DragaliaAPI.Test.AutoMapper;

public class AutoMapperProfileTest
{
    public class AutoMapperTheoryData : TheoryData<Type>
    {
        public AutoMapperTheoryData()
        {
            IEnumerable<Type> types = typeof(Program)
                .Assembly.GetTypes()
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
