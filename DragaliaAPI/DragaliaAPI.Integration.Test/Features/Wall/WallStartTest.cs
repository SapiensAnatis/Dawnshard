using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.Wall;

public class WallStartTest : TestFixture
{
    public WallStartTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 2);
    }

    [Fact]
    public async Task Start_ReturnsExpectedResponse()
    {
        int element = 1;
        int wallLevel = 10;

        int wallId = Convert.ToInt32($"21601000{element}");
        int expectedWallBossParamId = Convert.ToInt32($"21601{element}0{wallLevel}");

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = wallId,
                    WallLevel = wallLevel
                }
            }
        );

        WallStartStartResponse response = (
            await Client.PostMsgpack<WallStartStartResponse>(
                "/wall_start/start",
                new WallStartStartRequest()
                {
                    WallId = wallId,
                    WallLevel = wallLevel,
                    PartyNo = 1
                }
            )
        ).Data;

        response.OddsInfo.Enemy.First().ParamId.Should().Be(expectedWallBossParamId);

        response
            .IngameWallData.Should()
            .BeEquivalentTo(new IngameWallData() { WallId = wallId, WallLevel = wallLevel });
    }

    [Fact]
    public async Task StartAssignUnit_ReturnsExpectedResponse()
    {
        int wallLevel = 15;
        int element = 2;

        int wallId = Convert.ToInt32($"21601000{element}");
        int expectedWallBossParamId = Convert.ToInt32($"21601{element}0{wallLevel}");

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = wallId,
                    WallLevel = wallLevel
                }
            }
        );

        WallStartStartAssignUnitResponse response = (
            await Client.PostMsgpack<WallStartStartAssignUnitResponse>(
                "/wall_start/start_assign_unit",
                new WallStartStartAssignUnitRequest()
                {
                    WallId = wallId,
                    WallLevel = wallLevel,
                    RequestPartySettingList = new List<PartySettingList>()
                }
            )
        ).Data;

        response.OddsInfo.Enemy.First().ParamId.Should().Be(expectedWallBossParamId);

        response
            .IngameWallData.Should()
            .BeEquivalentTo(new IngameWallData() { WallId = wallId, WallLevel = wallLevel });
    }
}
