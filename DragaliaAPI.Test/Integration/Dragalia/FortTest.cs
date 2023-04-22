using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class FortTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public FortTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        TestUtils.ApplyDateTimeAssertionOptions(thresholdSec: 2);
    }

    [Fact]
    public async Task GetData_ReturnsBuildingData()
    {
        // 2012-06-16 12:12:59
        DateTimeOffset start = DateTimeOffset.FromUnixTimeSeconds(1339848779);
        // 2029-11-09 13:15:43
        DateTimeOffset end = DateTimeOffset.FromUnixTimeSeconds(1888924543);
        // 2013-01-14 07:47:23
        DateTimeOffset income = DateTimeOffset.FromUnixTimeSeconds(1358149643);

        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.AxeDojo,
                Level = 10,
                PositionX = 10,
                PositionZ = 10,
                BuildStartDate = start,
                BuildEndDate = end,
                IsNew = false,
                LastIncomeDate = income, // Axe dojos don't make you money but let's pretend they do
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        (
            await this.client.PostMsgpack<FortGetDataData>(
                "/fort/get_data",
                new FortGetDataRequest()
            )
        ).data.build_list
            .Should()
            .ContainEquivalentOf(
                new BuildList()
                {
                    plant_id = FortPlants.TheHalidom,
                    level = 1,
                    fort_plant_detail_id = 10010101,
                    position_x = 16, // Default Halidom position
                    position_z = 17,
                    last_income_time = DateTime.UtcNow - DateTimeOffset.UnixEpoch,
                    is_new = false,
                    build_start_date = DateTimeOffset.UnixEpoch,
                    build_end_date = DateTimeOffset.UnixEpoch,
                },
                opts => opts.Excluding(x => x.build_id)
            )
            .And.ContainEquivalentOf(
                new BuildList()
                {
                    plant_id = FortPlants.AxeDojo,
                    level = 10,
                    position_x = 10,
                    position_z = 10,
                    build_start_date = start,
                    build_end_date = end,
                    fort_plant_detail_id = 10050410,
                    build_status = FortBuildStatus.Construction,
                    is_new = false,
                    remain_time = end - DateTimeOffset.UtcNow,
                    last_income_time = DateTimeOffset.UtcNow - income
                },
                opts => opts.Excluding(x => x.build_id)
            );

        // Not much point asserting against the other properties since they're stubs
    }

    [Fact]
    public async Task AddCarpenter_ReturnsValidResult()
    {
        FortAddCarpenterData response = (
            await client.PostMsgpack<FortAddCarpenterData>(
                "/fort/add_carpenter",
                new FortAddCarpenterRequest(PaymentTypes.Wyrmite)
            )
        ).data;

        response.fort_detail.carpenter_num.Should().Be(3);
        response.update_data_list.user_data.crystal.Should().Be(1199750);
    }

    [Fact]
    public async Task BuildAtOnce_ReturnsValidResult()
    {
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 1,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        FortBuildAtOnceData response = (
            await client.PostMsgpack<FortBuildAtOnceData>(
                "/fort/build_at_once",
                new FortBuildAtOnceRequest(build.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        // The level changes when building starts, not when it ends, so no need to check it here
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task BuildCancel_ReturnsValidResult()
    {
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 2,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        FortBuildCancelData response = (
            await client.PostMsgpack<FortBuildCancelData>(
                "/fort/build_cancel",
                new FortBuildCancelRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(1); // Level should have decreased
    }

    [Fact]
    public async Task BuildEnd_ReturnsValidResult()
    {
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 1,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1682110410),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1682110411),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        FortBuildEndData response = (
            await client.PostMsgpack<FortBuildEndData>(
                "/fort/build_end",
                new FortBuildEndRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task BuildStart_ReturnsValidResult()
    {
        int ExpectedPositionX = 2;
        int ExpectedPositionZ = 2;

        FortBuildStartData response = (
            await client.PostMsgpack<FortBuildStartData>(
                "/fort/build_start",
                new FortBuildStartRequest(
                    FortPlants.FlameAltar,
                    ExpectedPositionX,
                    ExpectedPositionZ
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First();
        result.position_x.Should().Be(ExpectedPositionX);
        result.position_z.Should().Be(ExpectedPositionZ);
        result.build_start_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().BeAfter(result.build_start_date);
        response.fort_detail.working_carpenter_num.Should().Be(1);
    }

    [Fact]
    public async Task LevelupAtOnce_ReturnsValidResult()
    {
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 1,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupAtOnceData response = (
            await client.PostMsgpack<FortLevelupAtOnceData>(
                "/fort/levelup_at_once",
                new FortLevelupAtOnceRequest(build.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task LevelUpCancel_ReturnsValidResult()
    {
        
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 2,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupCancelData response = (
            await client.PostMsgpack<FortLevelupCancelData>(
                "/fort/levelup_cancel",
                new FortLevelupCancelRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(1); // Level should have decreased
    }

    [Fact]
    public async Task LevelUpEnd_ReturnsValidResult()
    {
        
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 1,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupEndData response = (
            await client.PostMsgpack<FortLevelupEndData>(
                "/fort/levelup_end",
                new FortLevelupEndRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task LevelUpStart_ReturnsValidResult()
    {
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 2,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = DateTimeOffset.UnixEpoch,
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupStartData response = (
            await client.PostMsgpack<FortLevelupStartData>(
                "/fort/levelup_start",
                new FortLevelupStartRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().BeAfter(result.build_start_date);
    }

    [Fact]
    public async Task Move_ReturnsValidResult()
    {
        DbFortBuild build = this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 1,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        ).Entity;
        await this.fixture.ApiContext.SaveChangesAsync();

        int ExpectedPositionX = 4;
        int ExpectedPositionZ = 4;
        FortMoveData response = (
            await client.PostMsgpack<FortMoveData>(
                "/fort/move",
                new FortMoveRequest(build.BuildId, ExpectedPositionX, ExpectedPositionZ)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.position_x.Should().Be(ExpectedPositionX);
        result.position_z.Should().Be(ExpectedPositionZ);
    }
}
