using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.Stamp;

public class StampTest : TestFixture
{
    private const string Controller = "/stamp";

    public StampTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetStamp_ReturnsStamps()
    {
        StampGetStampResponse data = (
            await this.Client.PostMsgpack<StampGetStampResponse>($"{Controller}/get_stamp")
        ).Data;

        data.StampList.Should().HaveCount(123);
    }

    [Fact]
    public async Task SetEquipStamp_UpdatesDatabase()
    {
        List<EquipStampList> requestList =
            new()
            {
                new() { Slot = 1, StampId = 10001 },
                new() { Slot = 2, StampId = 10004 },
                new() { Slot = 3, StampId = 10003 },
                new() { Slot = 4, StampId = 10002 },
                new() { Slot = 5, StampId = 10005 },
                new() { Slot = 6, StampId = 10303 },
                new() { Slot = 7, StampId = 10007 },
                new() { Slot = 8, StampId = 10008 },
                new() { Slot = 9, StampId = 10009 },
                new() { Slot = 10, StampId = 10010 },
                new() { Slot = 11, StampId = 10011 },
                new() { Slot = 12, StampId = 10012 },
                new() { Slot = 13, StampId = 10013 },
                new() { Slot = 14, StampId = 10014 },
                new() { Slot = 15, StampId = 10015 },
                new() { Slot = 16, StampId = 10016 },
                new() { Slot = 17, StampId = 10017 },
                new() { Slot = 18, StampId = 10018 },
                new() { Slot = 19, StampId = 10019 },
                new() { Slot = 20, StampId = 10020 },
                new() { Slot = 21, StampId = 10021 },
                new() { Slot = 22, StampId = 10022 },
                new() { Slot = 23, StampId = 10023 },
                new() { Slot = 24, StampId = 10024 },
                new() { Slot = 25, StampId = 10025 },
                new() { Slot = 26, StampId = 10026 },
                new() { Slot = 27, StampId = 10027 },
                new() { Slot = 28, StampId = 10028 },
                new() { Slot = 29, StampId = 10029 },
                new() { Slot = 30, StampId = 10030 },
                new() { Slot = 31, StampId = 10031 },
                new() { Slot = 32, StampId = 10201 }
            };

        await this.Client.PostMsgpack<StampSetEquipStampResponse>(
            $"{Controller}/set_equip_stamp",
            new StampSetEquipStampRequest() { StampList = requestList }
        );

        this.ApiContext.ChangeTracker.Clear();

        this.ApiContext.EquippedStamps.Where(x => x.ViewerId == ViewerId)
            .Should()
            .BeEquivalentTo(
                requestList,
                opts =>
                    opts.ExcludingMissingMembers()
                        .WithMapping<DbEquippedStamp>(dto => dto.Slot, db => db.Slot)
                        .WithMapping<DbEquippedStamp>(dto => dto.StampId, db => db.StampId)
            );
    }

    [Fact]
    public async Task SetEquipStamp_ReturnsCorrectResponse()
    {
        List<EquipStampList> requestList =
            new()
            {
                new() { Slot = 1, StampId = 10001 },
                new() { Slot = 2, StampId = 10004 },
                new() { Slot = 3, StampId = 10003 },
                new() { Slot = 4, StampId = 10002 },
                new() { Slot = 5, StampId = 10005 },
                new() { Slot = 6, StampId = 10303 },
                new() { Slot = 7, StampId = 10007 },
                new() { Slot = 8, StampId = 10008 },
                new() { Slot = 9, StampId = 10009 },
                new() { Slot = 10, StampId = 10010 },
                new() { Slot = 11, StampId = 10011 },
                new() { Slot = 12, StampId = 10012 },
                new() { Slot = 13, StampId = 10013 },
                new() { Slot = 14, StampId = 10014 },
                new() { Slot = 15, StampId = 10015 },
                new() { Slot = 16, StampId = 10016 },
                new() { Slot = 17, StampId = 10017 },
                new() { Slot = 18, StampId = 10018 },
                new() { Slot = 19, StampId = 10019 },
                new() { Slot = 20, StampId = 10020 },
                new() { Slot = 21, StampId = 10021 },
                new() { Slot = 22, StampId = 10022 },
                new() { Slot = 23, StampId = 10023 },
                new() { Slot = 24, StampId = 10024 },
                new() { Slot = 25, StampId = 10025 },
                new() { Slot = 26, StampId = 10026 },
                new() { Slot = 27, StampId = 10027 },
                new() { Slot = 28, StampId = 10028 },
                new() { Slot = 29, StampId = 10029 },
                new() { Slot = 30, StampId = 10030 },
                new() { Slot = 31, StampId = 10031 },
                new() { Slot = 32, StampId = 10201 }
            };

        StampSetEquipStampResponse data = (
            await this.Client.PostMsgpack<StampSetEquipStampResponse>(
                $"{Controller}/set_equip_stamp",
                new StampSetEquipStampRequest() { StampList = requestList }
            )
        ).Data;

        data.Should()
            .BeEquivalentTo(new StampSetEquipStampResponse() { EquipStampList = requestList });
    }
}
