using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Database.Test.Entities;

public class DbPlayerCharaDataTest
{
    private readonly ICharaDataService charaDataService;

    public DbPlayerCharaDataTest()
    {
        this.charaDataService = new CharaDataService();
    }

    [Fact]
    public void ManaCirclePieceIdList_Set_SetsExpectedValue()
    {
        DbPlayerCharaData chara = DbPlayerCharaDataFactory.Create(
            "id",
            this.charaDataService.GetData(Charas.GalaNedrick)
        );

        var input = new SortedSet<int>(Enumerable.Range(1, 50));

        chara.ManaCirclePieceIdList = input;
        chara.LimitBreakCount = 5;
        chara.ManaCirclePieceIdList.Should().BeEquivalentTo(input);
    }
}
