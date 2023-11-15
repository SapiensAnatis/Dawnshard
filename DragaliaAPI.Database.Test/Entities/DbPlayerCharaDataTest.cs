using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Test.Entities;

public class DbPlayerCharaDataTest
{
    [Fact]
    public void ManaCirclePieceIdList_Set_SetsExpectedValue()
    {
        DbPlayerCharaData chara = new("id", Charas.GalaNedrick);

        SortedSet<int> input = new(Enumerable.Range(1, 50));

        chara.ManaCirclePieceIdList = input;
        chara.LimitBreakCount = 5;
        chara.ManaCirclePieceIdList.Should().BeEquivalentTo(input);
    }
}
