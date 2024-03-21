using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V2UpdateTest : SavefileUpdateTestFixture
{
    public V2UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V2Update_AddsStampList()
    {
        LoadIndexResponse data = (
            await this.Client.PostMsgpack<LoadIndexResponse>("/load/index")
        ).Data;

        List<EquipStampList> expectedStampList =
            new()
            {
                new() { Slot = 1, StampId = 0 },
                new() { Slot = 2, StampId = 0 },
                new() { Slot = 3, StampId = 0 },
                new() { Slot = 4, StampId = 0 },
                new() { Slot = 5, StampId = 0 },
                new() { Slot = 6, StampId = 0 },
                new() { Slot = 7, StampId = 0 },
                new() { Slot = 8, StampId = 0 },
                new() { Slot = 9, StampId = 0 },
                new() { Slot = 10, StampId = 0 },
                new() { Slot = 11, StampId = 0 },
                new() { Slot = 12, StampId = 0 },
                new() { Slot = 13, StampId = 0 },
                new() { Slot = 14, StampId = 0 },
                new() { Slot = 15, StampId = 0 },
                new() { Slot = 16, StampId = 0 },
                new() { Slot = 17, StampId = 0 },
                new() { Slot = 18, StampId = 0 },
                new() { Slot = 19, StampId = 0 },
                new() { Slot = 20, StampId = 0 },
                new() { Slot = 21, StampId = 0 },
                new() { Slot = 22, StampId = 0 },
                new() { Slot = 23, StampId = 0 },
                new() { Slot = 24, StampId = 0 },
                new() { Slot = 25, StampId = 0 },
                new() { Slot = 26, StampId = 0 },
                new() { Slot = 27, StampId = 0 },
                new() { Slot = 28, StampId = 0 },
                new() { Slot = 29, StampId = 0 },
                new() { Slot = 30, StampId = 0 },
                new() { Slot = 31, StampId = 0 },
                new() { Slot = 32, StampId = 0 },
            };

        data.EquipStampList.Should().BeEquivalentTo(expectedStampList);

        this.ApiContext.EquippedStamps.Where(x => x.ViewerId == ViewerId)
            .Should()
            .BeEquivalentTo(
                expectedStampList,
                opts =>
                    opts.ExcludingMissingMembers()
                        .WithMapping<DbEquippedStamp>(dto => dto.Slot, db => db.Slot)
                        .WithMapping<DbEquippedStamp>(dto => dto.StampId, db => db.StampId)
            );
        (await this.ApiContext.Players.FindAsync(ViewerId))!
            .SavefileVersion.Should()
            .Be(MaxVersion);
    }
}
