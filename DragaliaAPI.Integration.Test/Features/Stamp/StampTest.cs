using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Integration.Test.Features.Stamp;

public class StampTest : TestFixture
{
    private const string Controller = "/stamp";

    public StampTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetStamp_ReturnsStamps()
    {
        StampGetStampData data = (
            await this.Client.PostMsgpack<StampGetStampData>(
                $"{Controller}/get_stamp",
                new StampGetStampRequest()
            )
        ).data;

        data.stamp_list.Should().HaveCount(123);
    }

    [Fact]
    public async Task SetEquipStamp_UpdatesDatabase()
    {
        List<EquipStampList> requestList =
            new()
            {
                new() { slot = 1, stamp_id = 10001 },
                new() { slot = 2, stamp_id = 10004 },
                new() { slot = 3, stamp_id = 10003 },
                new() { slot = 4, stamp_id = 10002 },
                new() { slot = 5, stamp_id = 10005 },
                new() { slot = 6, stamp_id = 10303 },
                new() { slot = 7, stamp_id = 10007 },
                new() { slot = 8, stamp_id = 10008 },
                new() { slot = 9, stamp_id = 10009 },
                new() { slot = 10, stamp_id = 10010 },
                new() { slot = 11, stamp_id = 10011 },
                new() { slot = 12, stamp_id = 10012 },
                new() { slot = 13, stamp_id = 10013 },
                new() { slot = 14, stamp_id = 10014 },
                new() { slot = 15, stamp_id = 10015 },
                new() { slot = 16, stamp_id = 10016 },
                new() { slot = 17, stamp_id = 10017 },
                new() { slot = 18, stamp_id = 10018 },
                new() { slot = 19, stamp_id = 10019 },
                new() { slot = 20, stamp_id = 10020 },
                new() { slot = 21, stamp_id = 10021 },
                new() { slot = 22, stamp_id = 10022 },
                new() { slot = 23, stamp_id = 10023 },
                new() { slot = 24, stamp_id = 10024 },
                new() { slot = 25, stamp_id = 10025 },
                new() { slot = 26, stamp_id = 10026 },
                new() { slot = 27, stamp_id = 10027 },
                new() { slot = 28, stamp_id = 10028 },
                new() { slot = 29, stamp_id = 10029 },
                new() { slot = 30, stamp_id = 10030 },
                new() { slot = 31, stamp_id = 10031 },
                new() { slot = 32, stamp_id = 10201 }
            };

        await this.Client.PostMsgpack<StampSetEquipStampData>(
            $"{Controller}/set_equip_stamp",
            new StampSetEquipStampRequest() { stamp_list = requestList }
        );

        this.ApiContext.ChangeTracker.Clear();

        this.ApiContext.EquippedStamps
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Should()
            .BeEquivalentTo(
                requestList,
                opts =>
                    opts.ExcludingMissingMembers()
                        .WithMapping<DbEquippedStamp>(dto => dto.slot, db => db.Slot)
                        .WithMapping<DbEquippedStamp>(dto => dto.stamp_id, db => db.StampId)
            );
    }

    [Fact]
    public async Task SetEquipStamp_ReturnsCorrectResponse()
    {
        List<EquipStampList> requestList =
            new()
            {
                new() { slot = 1, stamp_id = 10001 },
                new() { slot = 2, stamp_id = 10004 },
                new() { slot = 3, stamp_id = 10003 },
                new() { slot = 4, stamp_id = 10002 },
                new() { slot = 5, stamp_id = 10005 },
                new() { slot = 6, stamp_id = 10303 },
                new() { slot = 7, stamp_id = 10007 },
                new() { slot = 8, stamp_id = 10008 },
                new() { slot = 9, stamp_id = 10009 },
                new() { slot = 10, stamp_id = 10010 },
                new() { slot = 11, stamp_id = 10011 },
                new() { slot = 12, stamp_id = 10012 },
                new() { slot = 13, stamp_id = 10013 },
                new() { slot = 14, stamp_id = 10014 },
                new() { slot = 15, stamp_id = 10015 },
                new() { slot = 16, stamp_id = 10016 },
                new() { slot = 17, stamp_id = 10017 },
                new() { slot = 18, stamp_id = 10018 },
                new() { slot = 19, stamp_id = 10019 },
                new() { slot = 20, stamp_id = 10020 },
                new() { slot = 21, stamp_id = 10021 },
                new() { slot = 22, stamp_id = 10022 },
                new() { slot = 23, stamp_id = 10023 },
                new() { slot = 24, stamp_id = 10024 },
                new() { slot = 25, stamp_id = 10025 },
                new() { slot = 26, stamp_id = 10026 },
                new() { slot = 27, stamp_id = 10027 },
                new() { slot = 28, stamp_id = 10028 },
                new() { slot = 29, stamp_id = 10029 },
                new() { slot = 30, stamp_id = 10030 },
                new() { slot = 31, stamp_id = 10031 },
                new() { slot = 32, stamp_id = 10201 }
            };

        StampSetEquipStampData data = (
            await this.Client.PostMsgpack<StampSetEquipStampData>(
                $"{Controller}/set_equip_stamp",
                new StampSetEquipStampRequest() { stamp_list = requestList }
            )
        ).data;

        data.Should()
            .BeEquivalentTo(new StampSetEquipStampData() { equip_stamp_list = requestList });
    }
}
