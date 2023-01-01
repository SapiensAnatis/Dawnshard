using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Database.Test.Entities;

public class DbPlayerCharaDataTest
{
    [Fact]
    public void ManaCirclePieceIdList_Set_SetsExpectedValue()
    {
        DbPlayerCharaData chara = DbPlayerCharaDataFactory.Create(
            "id",
            MasterAsset.CharaData.Get(Charas.GalaNedrick)
        );

        SortedSet<int> input = new(Enumerable.Range(1, 50));

        chara.ManaCirclePieceIdList = input;
        chara.LimitBreakCount = 5;
        chara.ManaCirclePieceIdList.Should().BeEquivalentTo(input);
    }
}
