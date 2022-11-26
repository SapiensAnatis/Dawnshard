using AutoMapper;
using DragaliaAPI.Models.AutoMapper;

namespace DragaliaAPI.Test.Unit;

public class AutoMapperProfileTest
{
    [Fact]
    public void QuestMapProfile_IsValid()
    {
        TestProfile<QuestMapProfile>();
    }

    [Fact]
    public void InventoryMapProfile_IsValid()
    {
        TestProfile<InventoryMapProfile>();
    }

    [Fact]
    public void UnitMapProfile_IsValid()
    {
        TestProfile<UnitMapProfile>();
    }

    [Fact]
    public void PartyMapProfile_IsValid()
    {
        TestProfile<PartyMapProfile>();
    }

    [Fact]
    public void UserDataMapProfile_IsValid()
    {
        TestProfile<UserDataMapProfile>();
    }

    [Fact]
    public void SummonMapProfile_IsValid()
    {
        TestProfile<SummonMapProfile>();
    }

    private static void TestProfile<TProfile>() where TProfile : Profile, new()
    {
        MapperConfiguration config = new(cfg => cfg.AddProfile(new TProfile()));
        config.Invoking(x => x.AssertConfigurationIsValid()).Should().NotThrow();
    }
}
