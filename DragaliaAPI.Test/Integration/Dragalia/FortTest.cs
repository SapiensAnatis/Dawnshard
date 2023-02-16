using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentAssertions.Equivalency;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class FortTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public FortTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        //this.fixture.AddToDatabase(FortTestData.FortDetail).Wait();
        //this.fixture.AddRangeToDatabase(FortTestData.Builds).Wait();
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
                new FortAddCarpenterRequest(
                    (int)PaymentTypes.Wyrmite
                )
            )
        ).data;

        response.fort_detail.carpenter_num.Should().Be(3);
        response.update_data_list.user_data.crystal.Should().Be(1199750);
    }

    [Fact]
    public async Task BuildAtOnce_ReturnsValidResult()
    {
        ulong BuildId = 43452432344; // Comically large ID
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortBuildAtOnceData response = (
            await client.PostMsgpack<FortBuildAtOnceData>(
                "/fort/build_at_once",
                new FortBuildAtOnceRequest(
                    BuildId,
                    (int)PaymentTypes.Wyrmite
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        // The level changes when building starts, not when it ends, so no need to check it here
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task BuildCancel_ReturnsValidResult()
    {
        ulong BuildId = 43452432345;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortBuildCancelData response = (
            await client.PostMsgpack<FortBuildCancelData>(
                "/fort/build_cancel",
                new FortBuildCancelRequest(
                    BuildId
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(1); // Level should have decreased
    }

    [Fact]
    public async Task BuildEnd_ReturnsValidResult()
    {
        ulong BuildId = 43452432346;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortBuildEndData response = (
            await client.PostMsgpack<FortBuildEndData>(
                "/fort/build_end",
                new FortBuildEndRequest(
                    BuildId
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
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
        ulong BuildId = 43452432348;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupAtOnceData response = (
            await client.PostMsgpack<FortLevelupAtOnceData>(
                "/fort/levelup_at_once",
                new FortLevelupAtOnceRequest(
                    BuildId,
                    (int)PaymentTypes.Wyrmite
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task LevelUpCancel_ReturnsValidResult()
    {
        ulong BuildId = 43452432349;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupCancelData response = (
            await client.PostMsgpack<FortLevelupCancelData>(
                "/fort/levelup_cancel",
                new FortLevelupCancelRequest(
                    BuildId
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(1); // Level should have decreased
    }

    [Fact]
    public async Task LevelUpEnd_ReturnsValidResult()
    {
        ulong BuildId = 43452432350;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupEndData response = (
            await client.PostMsgpack<FortLevelupEndData>(
                "/fort/levelup_end",
                new FortLevelupEndRequest(
                    BuildId
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task LevelUpStart_ReturnsValidResult()
    {
        ulong BuildId = 43452432351;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
                DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                PlantId = FortPlants.StaffDojo,
                Level = 1,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.UnixEpoch,
                BuildEndDate = DateTimeOffset.UnixEpoch,
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        FortLevelupStartData response = (
            await client.PostMsgpack<FortLevelupStartData>(
                "/fort/levelup_start",
                new FortLevelupStartRequest(
                    BuildId
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        result.build_start_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().BeAfter(result.build_start_date);
        response.fort_detail.working_carpenter_num.Should().Be(2);
    }

    [Fact]
    public async Task Move_ReturnsValidResult()
    {
        ulong BuildId = 43452432362;
        this.fixture.ApiContext.PlayerFortBuilds.Add(
            new()
            {
                BuildId = (long)BuildId,
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
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        int ExpectedPositionX = 4;
        int ExpectedPositionZ = 4;
        FortMoveData response = (
            await client.PostMsgpack<FortMoveData>(
                "/fort/move",
                new FortMoveRequest(
                    BuildId,
                    ExpectedPositionX,
                    ExpectedPositionZ
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(x => x.build_id == BuildId);
        result.position_x.Should().Be(ExpectedPositionX);
        result.position_z.Should().Be(ExpectedPositionZ);
    }

    public static class FortTestData {
        public static readonly List<DbFortBuild> Builds =
            new()
            {
                new() {
                    // No state
                    DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 1,
                    PositionZ = 1,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch,
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                },
                new() {
                    // In Construction
                    DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                },
                new() {
                    // Finished
                    DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 3,
                    PositionZ = 3,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1339848779),
                    BuildEndDate = DateTimeOffset.UtcNow,
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            };
    }
}
