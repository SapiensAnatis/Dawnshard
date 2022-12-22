using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Shared.Test;

public class DragonDataServiceTest
{
    private readonly IDragonDataService dragonDataService;

    public DragonDataServiceTest()
    {
        this.dragonDataService = new DragonDataService();
    }

    [Fact]
    public void Get_ReturnsCorrectData()
    {
        dragonDataService.GetData(Dragons.Agni).FullName.Should().Be("Agni");
    }

    [Fact]
    public void Get_AllIdsValid()
    {
        foreach (Dragons d in Enum.GetValues<Dragons>())
        {
            dragonDataService.Invoking(x => x.GetData(d)).Should().NotThrow();
        }
    }

    [Fact]
    public void Get_InvalidId_ThrowsKeyNotFoundException()
    {
        dragonDataService.Invoking(x => x.GetData(0)).Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void Data_CanFilter()
    {
        dragonDataService.AllData
            .Where(x => x.FullName == "Agni")
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(dragonDataService.GetData(Dragons.Agni));
    }
}
