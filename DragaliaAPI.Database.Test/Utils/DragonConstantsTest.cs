using DragaliaAPI.Database.Utils;

namespace DragaliaAPI.Database.Test.Utils;

public class DragonConstantsTest
{
    [Theory]
    [InlineData(3, 0, 20)]
    [InlineData(3, 2, 40)]
    [InlineData(3, 4, 60)]
    [InlineData(4, 0, 30)]
    [InlineData(4, 2, 50)]
    [InlineData(4, 4, 80)]
    [InlineData(5, 0, 40)]
    [InlineData(5, 2, 70)]
    [InlineData(5, 4, 100)]
    [InlineData(5, 5, 120)]
    public void GetMaxLevelFor_CheckCorrectLevels(int rarity, int limitBreak, byte returnValue)
    {
        DragonConstants.GetMaxLevelFor(rarity, limitBreak).Should().Be(returnValue);
    }
}
