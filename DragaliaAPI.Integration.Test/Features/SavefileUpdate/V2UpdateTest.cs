using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V2UpdateTest : SavefileUpdateTestFixture
{
    public V2UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.ApiContext.EquippedStamps.ExecuteDelete();
    }

    [Fact]
    public async Task V2Update_AddsStampList()
    {
        LoadIndexData data = (
            await this.Client.PostMsgpack<LoadIndexData>("/load/index", new LoadIndexRequest())
        ).data;

        List<EquipStampList> expectedStampList =
            new()
            {
                new() { slot = 1, stamp_id = 0 },
                new() { slot = 2, stamp_id = 0 },
                new() { slot = 3, stamp_id = 0 },
                new() { slot = 4, stamp_id = 0 },
                new() { slot = 5, stamp_id = 0 },
                new() { slot = 6, stamp_id = 0 },
                new() { slot = 7, stamp_id = 0 },
                new() { slot = 8, stamp_id = 0 },
                new() { slot = 9, stamp_id = 0 },
                new() { slot = 10, stamp_id = 0 },
                new() { slot = 11, stamp_id = 0 },
                new() { slot = 12, stamp_id = 0 },
                new() { slot = 13, stamp_id = 0 },
                new() { slot = 14, stamp_id = 0 },
                new() { slot = 15, stamp_id = 0 },
                new() { slot = 16, stamp_id = 0 },
                new() { slot = 17, stamp_id = 0 },
                new() { slot = 18, stamp_id = 0 },
                new() { slot = 19, stamp_id = 0 },
                new() { slot = 20, stamp_id = 0 },
                new() { slot = 21, stamp_id = 0 },
                new() { slot = 22, stamp_id = 0 },
                new() { slot = 23, stamp_id = 0 },
                new() { slot = 24, stamp_id = 0 },
                new() { slot = 25, stamp_id = 0 },
                new() { slot = 26, stamp_id = 0 },
                new() { slot = 27, stamp_id = 0 },
                new() { slot = 28, stamp_id = 0 },
                new() { slot = 29, stamp_id = 0 },
                new() { slot = 30, stamp_id = 0 },
                new() { slot = 31, stamp_id = 0 },
                new() { slot = 32, stamp_id = 0 },
            };

        data.equip_stamp_list.Should().BeEquivalentTo(expectedStampList);

        this.ApiContext.EquippedStamps
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Should()
            .BeEquivalentTo(
                expectedStampList,
                opts =>
                    opts.ExcludingMissingMembers()
                        .WithMapping<DbEquippedStamp>(dto => dto.slot, db => db.Slot)
                        .WithMapping<DbEquippedStamp>(dto => dto.stamp_id, db => db.StampId)
            );
        (await this.ApiContext.Players.FindAsync(DeviceAccountId))!.SavefileVersion
            .Should()
            .Be(MaxVersion);
    }
}
