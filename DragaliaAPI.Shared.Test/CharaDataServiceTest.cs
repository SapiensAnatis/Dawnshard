using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Shared.Test;

public class CharaDataServiceTest
{
    private readonly ICharaDataService charaDataService;

    public CharaDataServiceTest()
    {
        this.charaDataService = new CharaDataService();
    }

    [Fact]
    public void Get_ReturnsCorrectData()
    {
        charaDataService.GetData(Charas.Ilia).FullName.Should().Be("Ilia");
    }

    [Fact]
    public void Get_AllIdsValid()
    {
        foreach (Charas c in Enum.GetValues<Charas>())
        {
            charaDataService.Invoking(x => x.GetData(c)).Should().NotThrow();
        }
    }

    [Fact]
    public void Get_InvalidId_ThrowsKeyNotFoundException()
    {
        charaDataService.Invoking(x => x.GetData(0)).Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void Data_CanFilter()
    {
        charaDataService.AllData
            .Where(x => x.FullName == "Ilia")
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(charaDataService.GetData(Charas.Ilia));
    }
}
