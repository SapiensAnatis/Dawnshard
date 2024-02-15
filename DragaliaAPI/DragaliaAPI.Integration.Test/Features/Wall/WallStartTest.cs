using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

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

        WallStartStartData response = (
            await Client.PostMsgpack<WallStartStartData>(
                "/wall_start/start",
                new WallStartStartRequest()
                {
                    wall_id = wallId,
                    wall_level = wallLevel,
                    party_no = 1
                }
            )
        ).data;

        response.odds_info.enemy.First().param_id.Should().Be(expectedWallBossParamId);

        response
            .ingame_wall_data.Should()
            .BeEquivalentTo(new IngameWallData() { wall_id = wallId, wall_level = wallLevel });
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

        WallStartStartAssignUnitData response = (
            await Client.PostMsgpack<WallStartStartAssignUnitData>(
                "/wall_start/start",
                new WallStartStartAssignUnitRequest()
                {
                    wall_id = wallId,
                    wall_level = wallLevel,
                    request_party_setting_list = new List<PartySettingList>()
                }
            )
        ).data;

        response.odds_info.enemy.First().param_id.Should().Be(expectedWallBossParamId);

        response
            .ingame_wall_data.Should()
            .BeEquivalentTo(new IngameWallData() { wall_id = wallId, wall_level = wallLevel });
    }
}
