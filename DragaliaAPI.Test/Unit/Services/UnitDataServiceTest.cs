using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Services.Data;
using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Data.Entity;

namespace DragaliaAPI.Test.Unit.Services;

public class UnitDataServiceTest
{
    private readonly IUnitDataService unitDataService;

    public UnitDataServiceTest()
    {
        this.unitDataService = new UnitDataService();
    }

    [Fact]
    public void Get_ReturnsCorrectData()
    {
        unitDataService.GetData(Charas.Ilia).FullName.Should().Be("Ilia");
    }

    [Fact]
    public void Get_AllIdsValid()
    {
        foreach (Charas c in Enum.GetValues<Charas>())
        {
            unitDataService.Invoking(x => x.GetData(c)).Should().NotThrow();
        }
    }

    [Fact]
    public void Get_InvalidId_ThrowsKeyNotFoundException()
    {
        unitDataService.Invoking(x => x.GetData(0)).Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void Data_CanFilter()
    {
        unitDataService.AllData
            .Where(x => x.FullName == "Ilia")
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(unitDataService.GetData(Charas.Ilia));
    }
}
